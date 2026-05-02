using UnityEngine;

public class RangedEnemy : EnemyBase
{
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float fireRate = 1.5f;
    public float projectileSpeed = 10f;

    private float fireTimer;

    public override void Initialize()
    {
        fireTimer = 0f;
    }

    void Update()
    {
        fireTimer += Time.deltaTime;

        if (fireTimer >= fireRate)
        {
            Shoot();
            fireTimer = 0f;
        }
    }

    void Shoot()
    {
        if (projectilePrefab == null) return;

        Transform spawnPoint = firePoint != null ? firePoint : transform;

        GameObject projectile = Instantiate(
            projectilePrefab,
            spawnPoint.position,
            spawnPoint.rotation
        );

        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = spawnPoint.forward * projectileSpeed;
        }
    }
}

