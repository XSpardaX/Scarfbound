using System;
using TMPro;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public float invincibilityDuration = 0.5f;

    public GameObject wispDropPrefab;
    public TextMeshProUGUI wispsHeld;
    public TextMeshProUGUI livesHeld;
    public int wispsToDropOnHit = 10;
    public float dropSpreadRadius = 2f;

    private bool isInvincible;
    private float invincibilityTimer;

    public int CurrentWisps => GameState.Instance.Wisps;
    public int CurrentLives => GameState.Instance.Lives;
    public bool IsInvincible => isInvincible;

    public event Action<int> OnWispsUpdated;
    public event Action<int> OnLivesUpdated;
    public event Action OnDamageTaken;
    public event Action OnDeath;

    private void Awake()
    {
        // Refresh UI on scene load with whatever's persisted in GameState.
        UpdateWispsUI(GameState.Instance.Wisps);
        UpdateLivesUI(GameState.Instance.Lives);
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
    }

    private void Update()
    {
        if (!isInvincible) return;

        invincibilityTimer -= Time.deltaTime;

        if (invincibilityTimer <= 0f)
        {
            isInvincible = false;
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

    public void AddWisps(int amount)
    {
        GameState.Instance.AddWisps(amount);
    }

    public void TakeDamage(int amount = 1)
    {
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

            DropWisps(currentWispCount);
            isInvincible = true;
            invincibilityTimer = invincibilityDuration;
        }
        else
        {
            Die();
        }
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

    public void Die()
    {
        if (OnDeath != null)
        {
            OnDeath.Invoke();
        }
    }

    public void SetInvincible(float duration)
    {
        isInvincible = true;
        invincibilityTimer = duration;
    }
}
