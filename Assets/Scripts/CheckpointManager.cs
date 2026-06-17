using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

// checkpoints, death, fade respawn
public class CheckpointManager : MonoBehaviour
{
    public static CheckpointManager Instance;
    public static bool IsRespawning { get; private set; }
    public static event System.Action OnPlayerRespawned;

    public Transform playerTransform;
    public PlayerHealth playerHealth;
    public CharacterController characterController;
    public TextMeshProUGUI livesRemaining;

    public float respawnInvincibilityDuration = 2f;
    public float deathFadeOutDuration = 0.5f;
    public float deathFadeInDuration = 0.5f;
    [FormerlySerializedAs("respawnMovementLockDuration")]
    [Tooltip("Total time the screen stays black before fading back in.")]
    public float respawnBlackHoldDuration = 2f;
    [Tooltip("How long before fade-in starts that the player is moved to the checkpoint.")]
    public float respawnBeforeFadeInDuration = 0.2f;

    private Stack<Vector3> checkpointStack = new Stack<Vector3>();

    private bool hasTouchedCheckpoint;
    private bool isRespawning;

    // scene load setup
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetStaticState()
    {
        IsRespawning = false;
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void RegisterSceneCallbacks()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneLoaded += OnSceneLoaded;
        OnSceneLoaded(SceneManager.GetActiveScene(), LoadSceneMode.Single);
    }

    private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ResetRespawnState();

        if (scene.name == "Main Menu")
        {
            return;
        }

        EnsureExistsForScene();
    }

    public static void ResetRespawnState()
    {
        IsRespawning = false;
    }

    public static void EnsureExistsForScene()
    {
        CheckpointManager existing = FindFirstObjectByType<CheckpointManager>();
        if (existing != null)
        {
            existing.PrepareForScene();
            return;
        }

        LevelManager levelManager = FindFirstObjectByType<LevelManager>();
        GameObject host = levelManager != null ? levelManager.gameObject : new GameObject("CheckpointManager");
        host.AddComponent<CheckpointManager>();
    }

    // start up
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        PrepareForScene();

        if (playerTransform != null)
        {
            checkpointStack.Push(playerTransform.position);
        }
    }

    public void PrepareForScene()
    {
        isRespawning = false;
        ResetRespawnState();
        StopAllCoroutines();
        ResolveReferences();
        SubscribeToPlayerDeath();
        ResetActivePlayerHealth();
    }

    private void ResetActivePlayerHealth()
    {
        if (playerHealth == null)
        {
            return;
        }

        playerHealth.ResetForScene();
    }

    private void OnEnable()
    {
        SubscribeToPlayerDeath();
        GameState.Instance.OnLivesChanged += UpdateLivesUI;
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }

        ResetRespawnState();
    }

    private void OnDisable()
    {
        if (playerHealth != null)
        {
            playerHealth.OnDeath -= HandleDeath;
        }

        if (GameState.Instance != null)
        {
            GameState.Instance.OnLivesChanged -= UpdateLivesUI;
        }
    }

    private void Start()
    {
        ResolveReferences();
        SubscribeToPlayerDeath();
        UpdateLivesUI(GameState.Instance.Lives);
    }

    // find player and UI
    private void ResolveReferences()
    {
        if (playerTransform == null || playerHealth == null || characterController == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject != null)
            {
                if (playerTransform == null) playerTransform = playerObject.transform;
                if (playerHealth == null) playerHealth = playerObject.GetComponent<PlayerHealth>();
                if (characterController == null) characterController = playerObject.GetComponent<CharacterController>();
            }
        }

        if (livesRemaining == null)
        {
            livesRemaining = FindUiText("Lives");
        }
    }

    private void SubscribeToPlayerDeath()
    {
        if (playerHealth == null) return;

        playerHealth.OnDeath -= HandleDeath;
        playerHealth.OnDeath += HandleDeath;
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

    private void UpdateLivesUI(int lives)
    {
        if (livesRemaining != null)
        {
            livesRemaining.text = "Lives: " + lives;
        }
    }

    // save spawn spots
    public void SetCheckpoint(Vector3 position)
    {
        checkpointStack.Push(position);
        hasTouchedCheckpoint = true;
    }

    // death and respawn flow
    private void HandleDeath()
    {
        if (isRespawning) return;

        ResolveReferences();
        if (playerHealth == null)
        {
            return;
        }

        StartCoroutine(RespawnRoutine());
    }

    private IEnumerator RespawnRoutine()
    {
        isRespawning = true;
        IsRespawning = true;

        try
        {
            yield return ScreenFader.FadeOut(deathFadeOutDuration);

            GameState.Instance.LoseLife();

            float blackWait = Mathf.Max(0f, respawnBlackHoldDuration - respawnBeforeFadeInDuration);
            yield return new WaitForSeconds(blackWait);

            if (GameState.Instance.Lives <= 0)
            {
                RespawnAtFirstCheckpoint();
                GameState.Instance.ResetForNewGame();
            }
            else
            {
                RespawnAtLatestCheckpoint();
            }

            yield return new WaitForSeconds(respawnBeforeFadeInDuration);
            yield return ScreenFader.FadeIn(deathFadeInDuration);
        }
        finally
        {
            isRespawning = false;
            IsRespawning = false;
        }
    }

    private void RespawnAtLatestCheckpoint()
    {
        Respawn(checkpointStack.Peek());
    }

    private void RespawnAtFirstCheckpoint()
    {
        Vector3 firstCheckpoint = checkpointStack.ToArray()[checkpointStack.Count - 1];
        Respawn(firstCheckpoint);
    }

    private void Respawn(Vector3 position)
    {
        if (playerTransform == null) return;

        if (characterController != null)
        {
            characterController.enabled = false;
            playerTransform.position = position;
            characterController.enabled = true;
        }
        else
        {
            playerTransform.position = position;
        }

        Player playerComponent = playerTransform.GetComponent<Player>();
        if (playerComponent != null)
        {
            playerComponent.ResetVerticalVelocity();
            playerComponent.ResetCamera();
        }

        if (playerHealth != null)
        {
            float invincibilityDuration = Mathf.Max(
                respawnInvincibilityDuration,
                respawnBeforeFadeInDuration + deathFadeInDuration);
            playerHealth.ReviveAfterRespawn();
            playerHealth.SetInvincible(invincibilityDuration);
        }

        if (OnPlayerRespawned != null)
        {
            OnPlayerRespawned.Invoke();
        }
    }
}
