using UnityEngine;

public class RangedEnemy : EnemyBase
{
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float fireRate = 1.5f;
    public float projectileSpeed = 10f;

    private float fireTimer;

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

        Transform spawnTransform;

        if (firePoint != null)
        {
            spawnTransform = firePoint;
        }
        else
        {
            spawnTransform = transform;
        }

        GameObject spawnedProjectile = Instantiate(
            projectilePrefab,
            spawnTransform.position,
            spawnTransform.rotation
        );

        Projectile projectileComponent = spawnedProjectile.GetComponent<Projectile>();
        if (projectileComponent != null)
        {
            projectileComponent.speed = projectileSpeed;
        }
    }
}
