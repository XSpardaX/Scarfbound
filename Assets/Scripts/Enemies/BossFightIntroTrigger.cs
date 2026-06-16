using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class BossFightIntroTrigger : MonoBehaviour
{
    public BossEnemy boss;
    public Transform cameraTransform;
    public Transform player;
    public Transform bossLookTarget;
    public bool deactivateBossUntilTriggered = true;
    public bool oneShot = true;
    public float cameraMoveDuration = 1f;
    public float introHoldDuration = 0.25f;
    public Vector3 cameraBossOffset = new Vector3(0f, 3.5f, -8f);

    private bool hasTriggered;
    private Vector3 cameraStartPosition;
    private Quaternion cameraStartRotation;
    private Collider triggerCollider;
    private PlayerHealth cachedPlayerHealth;

    private void Start()
    {
        triggerCollider = GetComponent<Collider>();

        if (boss == null)
        {
            boss = FindAnyObjectByType<BossEnemy>(FindObjectsInactive.Include);
        }

        if (player == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject != null)
            {
                player = playerObject.transform;
            }
        }

        if (cameraTransform == null && player != null)
        {
            Player playerController = player.GetComponent<Player>();
            if (playerController != null)
            {
                cameraTransform = playerController.cameraTransform;
            }
        }

        CacheAndSubscribePlayerHealth();

        if (boss != null && deactivateBossUntilTriggered)
        {
            boss.startFightOnAwake = false;
            boss.gameObject.SetActive(false);
        }
    }

    private void OnEnable()
    {
        CheckpointManager.OnPlayerRespawned += HandlePlayerRespawned;
    }

    private void OnDisable()
    {
        CheckpointManager.OnPlayerRespawned -= HandlePlayerRespawned;

        if (cachedPlayerHealth != null)
        {
            cachedPlayerHealth.OnDeath -= HandlePlayerDied;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (oneShot && hasTriggered) return;

        hasTriggered = true;
        StartCoroutine(PlayIntroSequence());
    }

    private IEnumerator PlayIntroSequence()
    {
        if (boss == null || cameraTransform == null || player == null)
        {
            yield break;
        }

        // Disable the trigger as soon as the boss is spawned so it can't be retriggered mid-fight.
        SetTriggerEnabled(false);

        DialogueState.isInDialogue = true;

        cameraStartPosition = cameraTransform.position;
        cameraStartRotation = cameraTransform.rotation;

        if (!boss.gameObject.activeSelf)
        {
            boss.gameObject.SetActive(true);
            yield return null;
        }

        Vector3 focusPoint = bossLookTarget != null ? bossLookTarget.position : boss.transform.position + Vector3.up * 2f;
        Vector3 targetCameraPosition = focusPoint + boss.transform.rotation * cameraBossOffset;
        Quaternion targetCameraRotation = Quaternion.LookRotation((focusPoint - targetCameraPosition).normalized, Vector3.up);

        yield return MoveCamera(cameraStartPosition, cameraStartRotation, targetCameraPosition, targetCameraRotation);

        float introDuration = boss.PlayIntroAttack();
        yield return new WaitForSeconds(introDuration + introHoldDuration);

        yield return MoveCamera(targetCameraPosition, targetCameraRotation, cameraStartPosition, cameraStartRotation);

        DialogueState.isInDialogue = false;
        boss.StartFight();

        if (oneShot)
        {
            // Keep this object alive (so it can re-enable after respawn),
            // but keep the trigger disabled until we reset.
            SetTriggerEnabled(false);
        }
    }

    private void HandlePlayerRespawned()
    {
        ResetBossFight();
    }

    private void HandlePlayerDied()
    {
        // Despawn immediately on death (not after respawn),
        // so the fight is truly "not going on" during the respawn sequence.
        ResetBossFight();
    }

    private void ResetBossFight()
    {
        StopAllCoroutines();
        DialogueState.isInDialogue = false;
        hasTriggered = false;

        if (boss == null)
        {
            boss = FindAnyObjectByType<BossEnemy>(FindObjectsInactive.Include);
        }

        if (boss == null) return;

        boss.ResetEncounter();
        CorruptibleWisp.RestoreAll();

        if (deactivateBossUntilTriggered)
        {
            boss.gameObject.SetActive(false);
        }

        // Bring the trigger back so the player can restart the fight after respawn.
        SetTriggerEnabled(true);
    }

    private void SetTriggerEnabled(bool enabled)
    {
        if (triggerCollider != null)
        {
            triggerCollider.enabled = enabled;
        }
    }

    private void CacheAndSubscribePlayerHealth()
    {
        if (cachedPlayerHealth != null)
        {
            cachedPlayerHealth.OnDeath -= HandlePlayerDied;
            cachedPlayerHealth = null;
        }

        if (player == null) return;

        cachedPlayerHealth = player.GetComponent<PlayerHealth>();
        if (cachedPlayerHealth != null)
        {
            cachedPlayerHealth.OnDeath -= HandlePlayerDied;
            cachedPlayerHealth.OnDeath += HandlePlayerDied;
        }
    }

    private IEnumerator MoveCamera(Vector3 fromPosition, Quaternion fromRotation, Vector3 toPosition, Quaternion toRotation)
    {
        float elapsed = 0f;

        while (elapsed < cameraMoveDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / cameraMoveDuration);
            cameraTransform.position = Vector3.Lerp(fromPosition, toPosition, t);
            cameraTransform.rotation = Quaternion.Slerp(fromRotation, toRotation, t);
            yield return null;
        }

        cameraTransform.position = toPosition;
        cameraTransform.rotation = toRotation;
    }
}
