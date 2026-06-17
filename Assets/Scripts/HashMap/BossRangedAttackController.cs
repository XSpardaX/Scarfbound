using UnityEngine;

public class BossRangedAttackController
{
    private readonly BossContext bossContext;
    private readonly BossEnemy bossSettings;
    private readonly BossAnimationController animationController;

    public BossRangedAttackController(BossContext context, BossEnemy bossEnemy, BossAnimationController bossAnimationController)
    {
        bossContext = context;
        bossSettings = bossEnemy;
        animationController = bossAnimationController;
    }

    public void Tick()
    {
        if (bossSettings.projectilePrefab == null)
        {
            return;
        }

        if (bossContext.IsAttacking)
        {
            return;
        }

        if (bossContext.State != BossBehaviorState.Active)
        {
            return;
        }

        bossContext.FireTimer += Time.deltaTime;

        float fireInterval = GetFireInterval();
        if (bossContext.FireTimer < fireInterval)
        {
            return;
        }

        bossContext.FireTimer = 0f;
        FireProjectiles();

        if (SfxManager.Instance != null)
        {
            SfxManager.Instance.PlayBossAttack();
        }

        animationController.PlayAttack();
    }

    public void ResetTimer()
    {
        bossContext.FireTimer = 0f;
    }

    private float GetFireInterval()
    {
        float horizontalDistanceToPlayer = GetHorizontalDistanceToPlayer();
        float normalizedDistance = Mathf.InverseLerp(
            bossSettings.minFireRateDistance,
            bossSettings.maxFireRateDistance,
            horizontalDistanceToPlayer);

        float baseFireInterval = Mathf.Lerp(
            bossSettings.closeFireInterval,
            bossSettings.farFireInterval,
            normalizedDistance);

        float finalFireInterval = baseFireInterval * bossContext.CurrentFireIntervalMultiplier;
        return finalFireInterval;
    }

    private float GetHorizontalDistanceToPlayer()
    {
        if (bossContext.Player == null)
        {
            return float.MaxValue;
        }

        Vector3 playerOffsetFromBoss = bossContext.Player.position - bossContext.Boss.transform.position;
        playerOffsetFromBoss.y = 0f;
        return playerOffsetFromBoss.magnitude;
    }

    private void FireProjectiles()
    {
        Vector3 projectileSpawnPosition;
        if (bossSettings.firePoint != null)
        {
            projectileSpawnPosition = bossSettings.firePoint.position;
        }
        else
        {
            projectileSpawnPosition = bossContext.Boss.transform.position + Vector3.up;
        }

        Quaternion baseProjectileRotation = GetProjectileRotation(projectileSpawnPosition);
        Vector3 convergenceDirection = baseProjectileRotation * Vector3.forward;

        bool shouldConvergeToCenter;
        float[] yawOffsets = GetYawOffsets(out shouldConvergeToCenter);

        foreach (float yawOffset in yawOffsets)
        {
            Quaternion projectileRotation = baseProjectileRotation * Quaternion.Euler(0f, yawOffset, 0f);
            GameObject spawnedProjectileObject = Object.Instantiate(
                bossSettings.projectilePrefab,
                projectileSpawnPosition,
                projectileRotation);

            Projectile spawnedProjectile = spawnedProjectileObject.GetComponent<Projectile>();
            if (spawnedProjectile == null)
            {
                continue;
            }

            spawnedProjectile.speed = bossContext.CurrentProjectileSpeed;
            spawnedProjectile.damage = bossSettings.projectileDamage;

            if (shouldConvergeToCenter)
            {
                spawnedProjectile.SetConvergence(
                    convergenceDirection,
                    bossSettings.phase2ConvergeStrength,
                    bossSettings.phase2ConvergeTurnRate);
            }
        }
    }

    private float[] GetYawOffsets(out bool shouldConverge)
    {
        if (bossContext.CurrentPhase == 2)
        {
            shouldConverge = true;
            return new[] { -bossSettings.phase2ProjectileYawOffset, bossSettings.phase2ProjectileYawOffset };
        }

        if (bossContext.CurrentPhase == 3)
        {
            if (bossContext.Phase3AlternateShot)
            {
                shouldConverge = false;
                bossContext.Phase3AlternateShot = false;

                return new[]
                {
                    0f,
                    bossSettings.phase3SideProjectileYawOffset,
                    360f - bossSettings.phase3SideProjectileYawOffset
                };
            }

            shouldConverge = true;
            bossContext.Phase3AlternateShot = true;
            return new[] { -bossSettings.phase2ProjectileYawOffset, bossSettings.phase2ProjectileYawOffset };
        }

        shouldConverge = false;
        return new[] { 0f };
    }

    private Quaternion GetProjectileRotation(Vector3 spawnPosition)
    {
        if (bossContext.Player == null)
        {
            return Quaternion.LookRotation(bossContext.Boss.transform.forward, Vector3.up);
        }

        Vector3 directionToPlayer = bossContext.Player.position - spawnPosition;
        directionToPlayer.y = 0f;

        if (directionToPlayer.sqrMagnitude < 0.0001f)
        {
            return Quaternion.LookRotation(bossContext.Boss.transform.forward, Vector3.up);
        }

        return Quaternion.LookRotation(directionToPlayer.normalized, Vector3.up);
    }
}
