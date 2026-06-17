using System;
using System.Collections;
using TMPro;
using UnityEngine;

// wisps block damage, die at zero wisps
public class PlayerHealth : MonoBehaviour
{
    public float invincibilityDuration = 0.5f;

    public GameObject wispDropPrefab;
    public TextMeshProUGUI wispsHeld;
    public TextMeshProUGUI livesHeld;
    public int wispsToDropOnHit = 10;
    public float dropSpreadRadius = 2f;
    public float damageFlickerInterval = 0.08f;

    private bool isInvincible;
    private float invincibilityTimer;
    private bool shouldPlayDamageFlicker;
    private bool isVisible = true;
    private bool isDead;
    private Coroutine damageFlickerRoutine;
    private Renderer[] playerRenderers;

    public int CurrentWisps => GameState.Instance.Wisps;
    public int CurrentLives => GameState.Instance.Lives;
    public bool IsInvincible => isInvincible;
    public bool IsDead => isDead;

    public event Action<int> OnWispsUpdated;
    public event Action<int> OnLivesUpdated;
    public event Action OnDamageTaken;
    public event Action OnDeath;

    // start up and HUD
    private void Awake()
    {
        playerRenderers = GetComponentsInChildren<Renderer>(true);
        ResolveReferences();

        UpdateWispsUI(GameState.Instance.Wisps);
        UpdateLivesUI(GameState.Instance.Lives);
    }

    private void Start()
    {
        ResolveReferences();
        UpdateWispsUI(GameState.Instance.Wisps);
        UpdateLivesUI(GameState.Instance.Lives);
    }

    private void ResolveReferences()
    {
        if (wispsHeld == null)
        {
            wispsHeld = FindUiText("Wisps held");
        }

        if (livesHeld == null)
        {
            livesHeld = FindUiText("Lives");
        }
    }

    private static TextMeshProUGUI FindUiText(string objectName)
    {
        TextMeshProUGUI[] labels = FindObjectsByType<TextMeshProUGUI>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        foreach (TextMeshProUGUI label in labels)
        {
            if (label.gameObject.name == objectName)
            {
                return label;
            }
        }

        return null;
    }

    private void OnEnable()
    {
        GameState.Instance.OnWispsChanged += UpdateWispsUI;
        GameState.Instance.OnLivesChanged += UpdateLivesUI;
    }

    private void OnDisable()
    {
        if (GameState.Instance == null) return;

        GameState.Instance.OnWispsChanged -= UpdateWispsUI;
        GameState.Instance.OnLivesChanged -= UpdateLivesUI;

        StopDamageFlicker();
        SetPlayerVisible(true);
    }

    private void Update()
    {
        if (!isInvincible) return;

        invincibilityTimer -= Time.deltaTime;

        if (invincibilityTimer <= 0f)
        {
            isInvincible = false;
            shouldPlayDamageFlicker = false;
            StopDamageFlicker();
            SetPlayerVisible(true);
        }
    }

    private void UpdateWispsUI(int wisps)
    {
        if (wispsHeld != null)
        {
            wispsHeld.text = "Wisps held: " + wisps;
        }

        if (OnWispsUpdated != null)
        {
            OnWispsUpdated.Invoke(wisps);
        }
    }

    private void UpdateLivesUI(int lives)
    {
        if (livesHeld != null)
        {
            livesHeld.text = "Lives: " + lives;
        }

        if (OnLivesUpdated != null)
        {
            OnLivesUpdated.Invoke(lives);
        }
    }

    // hurt, drop wisps
    public void AddWisps(int amount)
    {
        GameState.Instance.AddWisps(amount);
    }

    public void TakeDamage(int amount = 1)
    {
        if (isDead) return;
        if (isInvincible) return;
        if (amount <= 0) return;

        int currentWispCount = GameState.Instance.Wisps;

        if (currentWispCount > 0)
        {
            GameState.Instance.SetWisps(0);

            if (OnDamageTaken != null)
            {
                OnDamageTaken.Invoke();
            }

            if (SfxManager.Instance != null) SfxManager.Instance.Play(SfxIds.Hit);

            DropWisps(currentWispCount);
            isInvincible = true;
            invincibilityTimer = invincibilityDuration;
            shouldPlayDamageFlicker = true;
            StartDamageFlicker();
            return;
        }

        Die();
    }

    private void DropWisps(int count)
    {
        if (wispDropPrefab == null) return;

        for (int i = 0; i < count; i++)
        {
            Vector3 randomOffset = UnityEngine.Random.insideUnitSphere * dropSpreadRadius;
            randomOffset.y = Mathf.Abs(randomOffset.y) + 0.5f;

            Vector3 spawnPosition = transform.position + randomOffset;
            Instantiate(wispDropPrefab, spawnPosition, Quaternion.identity);
        }
    }

    // die and reset flags
    public void Die()
    {
        if (isDead)
        {
            return;
        }

        isDead = true;
        shouldPlayDamageFlicker = false;
        StopDamageFlicker();
        SetPlayerVisible(true);

        if (OnDeath != null)
        {
            OnDeath.Invoke();
        }
    }

    public void ReviveAfterRespawn()
    {
        isDead = false;
    }

    public void ResetForScene()
    {
        isDead = false;
        isInvincible = false;
        invincibilityTimer = 0f;
        shouldPlayDamageFlicker = false;
        StopDamageFlicker();
        SetPlayerVisible(true);
    }

    public void SetInvincible(float duration)
    {
        isInvincible = true;
        invincibilityTimer = duration;
        shouldPlayDamageFlicker = false;
        StopDamageFlicker();
        SetPlayerVisible(true);
    }

    // blink after hit
    private void StartDamageFlicker()
    {
        if (!shouldPlayDamageFlicker)
        {
            return;
        }

        if (damageFlickerRoutine != null)
        {
            StopCoroutine(damageFlickerRoutine);
        }

        damageFlickerRoutine = StartCoroutine(DamageFlickerRoutine());
    }

    private void StopDamageFlicker()
    {
        if (damageFlickerRoutine != null)
        {
            StopCoroutine(damageFlickerRoutine);
            damageFlickerRoutine = null;
        }
    }

    private IEnumerator DamageFlickerRoutine()
    {
        float flickerDelay = Mathf.Max(0.02f, damageFlickerInterval);

        while (isInvincible && shouldPlayDamageFlicker)
        {
            SetPlayerVisible(!isVisible);
            yield return new WaitForSeconds(flickerDelay);
        }

        SetPlayerVisible(true);
        damageFlickerRoutine = null;
    }

    private void SetPlayerVisible(bool visible)
    {
        isVisible = visible;

        if (playerRenderers == null)
        {
            return;
        }

        foreach (Renderer playerRenderer in playerRenderers)
        {
            if (playerRenderer == null)
            {
                continue;
            }

            playerRenderer.enabled = visible;
        }
    }
}
