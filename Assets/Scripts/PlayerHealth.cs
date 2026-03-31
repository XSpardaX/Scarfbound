using System;
using TMPro;
using UnityEngine;
using static Unity.Burst.Intrinsics.X86.Avx;

public class PlayerHealth : MonoBehaviour
{
    public float invincibilityDuration = 0.5f;

    public GameObject wispDropPrefab;
    public TextMeshProUGUI wispsHeld;
    public int wispsToDropOnHit = 10;
    public float dropSpreadRadius = 2f;

    private int currentWisps;
    private bool isInvincible;
    private float invincibilityTimer;

    public int CurrentWisps => currentWisps;

    public event Action<int> OnWispsUpdated;

    public event Action OnDamageTaken;

    public event Action OnDeath;

    private void Update()
    {
        if (isInvincible)
        {
            invincibilityTimer -= Time.deltaTime;
            if (invincibilityTimer <= 0f)
                isInvincible = false;
        }
        wispsHeld.text = "Wisps held: "+currentWisps;
    }

    public void AddWisps(int amount)
    {
        if (amount <= 0) return;
        currentWisps += amount;
        OnWispsUpdated?.Invoke(currentWisps);
    }

    public void TakeDamage(int amount = 1)
    {
        if (isInvincible) return;
        if (amount <= 0) return;

        if (currentWisps > 0)
        {
            int toDrop = currentWisps;
            currentWisps = 0;
            OnWispsUpdated?.Invoke(currentWisps);
            OnDamageTaken?.Invoke();

            DropWisps(toDrop);
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
            Vector3 offset = UnityEngine.Random.insideUnitSphere * dropSpreadRadius;
            offset.y = Mathf.Abs(offset.y) + 0.5f; 
            Vector3 spawnPos = transform.position + offset;
            GameObject wisp = Instantiate(wispDropPrefab, spawnPos, Quaternion.identity);
        }
    }

    public void Die()
    {
        OnDeath?.Invoke();
    }

    public bool IsInvincible => isInvincible;

    public void SetInvincible(float duration)
    {
        isInvincible = true;
        invincibilityTimer = duration;
    }

}
