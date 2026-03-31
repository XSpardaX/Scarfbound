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

    public int livesPerLevel = 3;
    private int currentLives;

    private Stack<Vector3> checkpointStack = new Stack<Vector3>();

    private bool hasTouchedCheckpoint;

    private bool isRespawning = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        currentLives = livesPerLevel;

        if (playerTransform != null)
        {
            checkpointStack.Push(playerTransform.position);
        }
    }

    private void Start()
    {
        livesRemaining.text = "Lives: " + currentLives;
    }

    public void SetCheckpoint(Vector3 position)
    {
        checkpointStack.Push(position);
        hasTouchedCheckpoint = true;
    }

    private void OnEnable()
    {
        if (playerHealth != null)
            playerHealth.OnDeath += HandleDeath;
    }

    private void OnDisable()
    {
        if (playerHealth != null)
            playerHealth.OnDeath -= HandleDeath;
    }

    private void HandleDeath()
    {
        if (isRespawning) return;
        StartCoroutine(RespawnRoutine());
    }

    private IEnumerator RespawnRoutine()
    {
        isRespawning = true;

        currentLives--;
        livesRemaining.text = "Lives: " + currentLives;

        if (currentLives <= 0)
        {
            RespawnAtFirstCheckpoint();
            currentLives = livesPerLevel;
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
        Vector3 first = checkpointStack.ToArray()[checkpointStack.Count - 1];
        Respawn(first);
    }

    private void Respawn(Vector3 position)
    {
        if (playerTransform == null) return;

        characterController.enabled = false;
        playerTransform.position = position;
        characterController.enabled = true;

        Player playerController = playerTransform.GetComponent<Player>();
        if (playerController != null)
            playerController.ResetVerticalVelocity();

        playerHealth?.SetInvincible(respawnInvincibilityDuration);
    }
}