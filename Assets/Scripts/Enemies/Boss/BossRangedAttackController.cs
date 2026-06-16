using UnityEngine;

public class BossRangedAttackController
{
    private readonly BossContext ctx;
    private readonly BossEnemy boss;
    private readonly BossAnimationController animation;

    public BossRangedAttackController(BossContext context, BossEnemy bossEnemy, BossAnimationController animationController)
    {
        ctx = context;
        boss = bossEnemy;
        animation = animationController;
    }

    public void Tick()
    {
        if (boss.projectilePrefab == null || ctx.IsAttacking || ctx.State != BossBehaviorState.Active) return;

        ctx.FireTimer += Time.deltaTime;
        if (ctx.FireTimer < GetFireInterval()) return;

        ctx.FireTimer = 0f;
        Fire();
        if (SfxManager.Instance != null) SfxManager.Instance.PlayBossAttack();
        animation.PlayAttack();
    }

    public void ResetTimer() => ctx.FireTimer = 0f;

    private float GetFireInterval()
    {
        float distanceT = Mathf.InverseLerp(boss.minFireRateDistance, boss.maxFireRateDistance, GetHorizontalDistance());
        return Mathf.Lerp(boss.closeFireInterval, boss.farFireInterval, distanceT) * ctx.CurrentFireIntervalMultiplier;
    }

    private float GetHorizontalDistance()
    {
        if (ctx.Player == null) return float.MaxValue;
        Vector3 offset = ctx.Player.position - ctx.Boss.transform.position;
        offset.y = 0f;
        return offset.magnitude;
    }

    private void Fire()
    {
        Vector3 spawnPos = boss.firePoint != null ? boss.firePoint.position : ctx.Boss.transform.position + Vector3.up;
        Quaternion baseRotation = GetProjectileRotation(spawnPos);
        Vector3 convergeDirection = baseRotation * Vector3.forward;
        float[] yawOffsets = GetYawOffsets(out bool shouldConverge);

        foreach (float yawOffset in yawOffsets)
        {
            Quaternion rotation = baseRotation * Quaternion.Euler(0f, yawOffset, 0f);
            GameObject spawned = Object.Instantiate(boss.projectilePrefab, spawnPos, rotation);
            Projectile projectile = spawned.GetComponent<Projectile>();
            if (projectile == null) continue;

            projectile.speed = ctx.CurrentProjectileSpeed;
            projectile.damage = boss.projectileDamage;
            if (shouldConverge) projectile.SetConvergence(convergeDirection, boss.phase2ConvergeStrength, boss.phase2ConvergeTurnRate);
        }
    }

    private float[] GetYawOffsets(out bool shouldConverge)
    {
        switch (ctx.CurrentPhase)
        {
            case 2:
                shouldConverge = true;
                return new[] { -boss.phase2ProjectileYawOffset, boss.phase2ProjectileYawOffset };
            case 3:
                if (ctx.Phase3AlternateShot)
                {
                    shouldConverge = false;
                    ctx.Phase3AlternateShot = false;
                    return new[] { 0f, boss.phase3SideProjectileYawOffset, 360f - boss.phase3SideProjectileYawOffset };
                }
                shouldConverge = true;
                ctx.Phase3AlternateShot = true;
                return new[] { -boss.phase2ProjectileYawOffset, boss.phase2ProjectileYawOffset };
            default:
                shouldConverge = false;
                return new[] { 0f };
        }
    }

    private Quaternion GetProjectileRotation(Vector3 spawnPosition)
    {
        if (ctx.Player == null) return Quaternion.LookRotation(ctx.Boss.transform.forward, Vector3.up);

        Vector3 directionToPlayer = ctx.Player.position - spawnPosition;
        directionToPlayer.y = 0f;
        return directionToPlayer.sqrMagnitude < 0.0001f
            ? Quaternion.LookRotation(ctx.Boss.transform.forward, Vector3.up)
            : Quaternion.LookRotation(directionToPlayer.normalized, Vector3.up);
    }
}
