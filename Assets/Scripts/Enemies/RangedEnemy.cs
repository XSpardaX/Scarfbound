using UnityEngine;

public class RangedEnemy : EnemyBase
{
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float fireRate = 1.5f;
    public float projectileSpeed = 10f;

    private float fireTimer;
    private Collider[] enemyColliders;

    private void Awake()
    {
        enemyColliders = GetComponentsInChildren<Collider>();
    }

    private void Update()
    {
        fireTimer += Time.deltaTime;

        if (fireTimer >= fireRate)
        {
            Shoot();
            fireTimer = 0f;
        }
    }

    private void Shoot()
    {
        if (projectilePrefab == null) return;

        Vector3 spawnPosition;
        Quaternion spawnRotation;

        if (firePoint != null)
        {
            spawnPosition = firePoint.position;
            spawnRotation = firePoint.rotation;
        }
        else
        {
            spawnPosition = transform.position + (transform.forward * 1.1f) + (Vector3.up * 0.5f);
            spawnRotation = transform.rotation;
        }

        GameObject spawnedProjectile = Instantiate(
            projectilePrefab,
            spawnPosition,
            spawnRotation
        );

        Collider projectileCollider = spawnedProjectile.GetComponent<Collider>();
        if (projectileCollider != null && enemyColliders != null)
        {
            foreach (Collider enemyCollider in enemyColliders)
            {
                if (enemyCollider == null) continue;
                Physics.IgnoreCollision(projectileCollider, enemyCollider);
            }
        }

        Projectile projectileComponent = spawnedProjectile.GetComponent<Projectile>();
        if (projectileComponent != null)
        {
            projectileComponent.speed = projectileSpeed;
        }
    }
}
