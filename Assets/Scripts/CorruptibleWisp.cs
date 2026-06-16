using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class CorruptibleWisp : MonoBehaviour
{
    private static readonly List<CorruptibleWisp> CorruptedWisps = new List<CorruptibleWisp>();

    public GameObject projectilePrefab;

    private Wisps collectible;
    private GameObject spawnedProjectile;
    private bool isCorrupted;
    private bool wasActive;

    private void Awake()
    {
        collectible = GetComponent<Wisps>();

        if (projectilePrefab == null)
        {
            BossEnemy boss = FindAnyObjectByType<BossEnemy>(FindObjectsInactive.Include);
            if (boss != null)
            {
                projectilePrefab = boss.projectilePrefab;
            }
        }
    }

    public bool TryCorrupt(Projectile sourceProjectile)
    {
        if (isCorrupted || !gameObject.activeInHierarchy || sourceProjectile == null) return false;
        if (projectilePrefab == null) return false;

        isCorrupted = true;
        wasActive = gameObject.activeSelf;
        CorruptedWisps.Add(this);

        if (collectible != null)
        {
            collectible.enabled = false;
        }

        Vector3 spawnPosition = transform.position;
        Quaternion spawnRotation = sourceProjectile.transform.rotation;

        spawnedProjectile = Instantiate(projectilePrefab, spawnPosition, spawnRotation);
        Projectile spawnedProjectileComponent = spawnedProjectile.GetComponent<Projectile>();
        if (spawnedProjectileComponent != null)
        {
            spawnedProjectileComponent.speed = sourceProjectile.speed;
            spawnedProjectileComponent.damage = sourceProjectile.damage;
        }

        SetWispVisuals(false);
        return true;
    }

    public void Restore()
    {
        if (!isCorrupted) return;

        isCorrupted = false;
        CorruptedWisps.Remove(this);

        if (spawnedProjectile != null)
        {
            Destroy(spawnedProjectile);
            spawnedProjectile = null;
        }

        if (collectible != null)
        {
            collectible.enabled = true;
        }

        SetWispVisuals(true);

        if (wasActive)
        {
            gameObject.SetActive(true);
        }
    }

    public static void RestoreAll()
    {
        for (int i = CorruptedWisps.Count - 1; i >= 0; i--)
        {
            if (CorruptedWisps[i] != null)
            {
                CorruptedWisps[i].Restore();
            }
        }

        CorruptedWisps.Clear();
    }

    private void SetWispVisuals(bool visible)
    {
        Renderer[] renderers = GetComponentsInChildren<Renderer>(true);
        foreach (Renderer renderer in renderers)
        {
            renderer.enabled = visible;
        }

        Collider[] colliders = GetComponentsInChildren<Collider>(true);
        foreach (Collider collider in colliders)
        {
            collider.enabled = visible;
        }
    }
}
