using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    public static CheckpointManager Instance;

    public Transform playerTransform;
    public PlayerHealth playerHealth;
    public CharacterController characterController;
    public TextMeshProUGUI livesRemaining;

    public float respawnInvincibilityDuration = 2f;

    private Stack<Vector3> checkpointStack = new Stack<Vector3>();

    private bool hasTouchedCheckpoint;
    private bool isRespawning;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        if (playerTransform != null)
        {
            checkpointStack.Push(playerTransform.position);
        }
    }

    private void OnEnable()
    {
        if (playerHealth != null)
        {
            playerHealth.OnDeath += HandleDeath;
        }

        GameState.Instance.OnLivesChanged += UpdateLivesUI;
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
        UpdateLivesUI(GameState.Instance.Lives);
    }

    private void UpdateLivesUI(int lives)
    {
        if (livesRemaining != null)
        {
            livesRemaining.text = "Lives: " + lives;
        }
    }

    public void SetCheckpoint(Vector3 position)
    {
        checkpointStack.Push(position);
        hasTouchedCheckpoint = true;
    }

    private void HandleDeath()
    {
        if (isRespawning) return;
        StartCoroutine(RespawnRoutine());
    }

    private IEnumerator RespawnRoutine()
    {
        isRespawning = true;

        GameState.Instance.LoseLife();

        if (GameState.Instance.Lives <= 0)
        {
            RespawnAtFirstCheckpoint();
            GameState.Instance.ResetForNewGame();
        }
        else
        {
            RespawnAtLatestCheckpoint();
        }

        yield return new WaitForSeconds(0.1f);

        isRespawning = false;
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

        characterController.enabled = false;
        playerTransform.position = position;
        characterController.enabled = true;

        Player playerComponent = playerTransform.GetComponent<Player>();
        if (playerComponent != null)
        {
            playerComponent.ResetVerticalVelocity();
        }

        if (playerHealth != null)
        {
            playerHealth.SetInvincible(respawnInvincibilityDuration);
        }
    }
}
