using UnityEngine;

public class BossPhaseController
{
    private readonly BossContext ctx;
    private readonly BossEnemy boss;
    private readonly BossMovementController movement;
    private readonly BossAnimationController animation;

    public BossPhaseController(
        BossContext context,
        BossEnemy bossEnemy,
        BossMovementController movementController,
        BossAnimationController animationController)
    {
        ctx = context;
        boss = bossEnemy;
        movement = movementController;
        animation = animationController;
    }

    public void ApplySettings()
    {
        switch (ctx.CurrentPhase)
        {
            case 1:
                ctx.Agent.speed = boss.phase1MoveSpeed;
                ctx.CurrentProjectileSpeed = boss.phase1ProjectileSpeed;
                ctx.CurrentFireIntervalMultiplier = boss.phase1FireIntervalMultiplier;
                ctx.Agent.updateRotation = !ctx.FightStarted;
                break;
            case 2:
                ctx.Agent.speed = boss.phase2MoveSpeed;
                ctx.CurrentProjectileSpeed = boss.phase2ProjectileSpeed;
                ctx.CurrentFireIntervalMultiplier = boss.phase2FireIntervalMultiplier;
                ctx.Agent.updateRotation = false;
                break;
            default:
                ctx.Agent.speed = boss.phase3MoveSpeed;
                ctx.CurrentProjectileSpeed = boss.phase3ProjectileSpeed;
                ctx.CurrentFireIntervalMultiplier = boss.phase3FireIntervalMultiplier;
                ctx.Agent.updateRotation = false;
                ctx.Phase3AlternateShot = false;
                break;
        }
    }

    public void StartFight()
    {
        if (ctx.FightStarted) return;

        ctx.FightStarted = true;
        ctx.State = BossBehaviorState.Active;
        ctx.CurrentPhase = 1;
        ctx.PhaseEndTime = Time.time + boss.phase1Duration;
        ctx.IsPhaseTimerPaused = false;
        ResetFireTimer();
        ctx.IsRunAnimPlaying = false;
        ApplySettings();
        ctx.Agent.isStopped = false;
        ctx.Patrol.ResumeFromCurrentPosition();
    }

    public void ResetEncounter()
    {
        SetHint(false);
        ctx.FightStarted = false;
        ctx.State = BossBehaviorState.Active;
        ctx.RecoveryStep = BossRecoveryStep.GetHit;
        ctx.IsRunAnimPlaying = ctx.IsAttacking = ctx.IsDeathStomp = ctx.IsPhaseTimerPaused = false;
        ctx.StompCount = 0;
        ctx.CurrentPhase = 1;
        ctx.Phase3AlternateShot = false;
        ResetFireTimer();
        SetBodySolid(false);
        ctx.Boss.transform.SetPositionAndRotation(ctx.SpawnPosition, ctx.SpawnRotation);
        ApplySettings();
        movement.Stop();
        animation.PlayIdle();
    }

    public void TickTimedStates()
    {
        if (ctx.State == BossBehaviorState.Recovering && Time.time >= ctx.RecoveryEndTime)
        {
            if (ctx.RecoveryStep == BossRecoveryStep.GetHit)
            {
                if (ctx.IsDeathStomp) BeginDeathSequence();
                else PlayDieRecoverAnimation();
            }
            else FinishRecovery();
        }
        else if (ctx.State == BossBehaviorState.Dying && Time.time >= ctx.RecoveryEndTime)
        {
            if (boss.keyPrefab != null)
                Object.Instantiate(boss.keyPrefab, ctx.Boss.transform.position + boss.keySpawnOffset, Quaternion.identity);
            Object.Destroy(ctx.Boss.gameObject);
        }
    }

    public void TickStagger()
    {
        if (ctx.State == BossBehaviorState.Staggered && Time.time >= ctx.StaggerEndTime)
            EndStaggerWithoutStomp();
    }

    public void TickPhaseTimer()
    {
        if (ctx.State == BossBehaviorState.Active && Time.time >= ctx.PhaseEndTime)
            EnterStagger();
    }

    public void ResetFireTimer() => ctx.FireTimer = 0f;

    public void EnterStagger()
    {
        if (ctx.State == BossBehaviorState.Staggered) return;

        ctx.State = BossBehaviorState.Staggered;
        SetBodySolid(true);
        ResetFireTimer();
        ctx.StaggerEndTime = Time.time + boss.staggerDuration;
        animation.CancelAttack();
        movement.Stop();
        ctx.Agent.updateRotation = true;
        animation.PlayDizzy();
        SetHint(true);
    }

    public void BeginHeadStompRecovery(Player touchingPlayer)
    {
        SetHint(false);
        ctx.StompCount++;
        ctx.IsDeathStomp = ctx.StompCount >= 3;
        ctx.State = BossBehaviorState.Recovering;
        SetBodySolid(true);
        ctx.RecoveryStep = BossRecoveryStep.GetHit;
        ctx.RecoveryEndTime = Time.time + boss.getHitAnimDuration;
        movement.Stop();
        ctx.Agent.updateRotation = true;
        animation.PlayGetHit();
        if (SfxManager.Instance != null) SfxManager.Instance.PlayBossHurt();
        touchingPlayer.ApplyBounce(boss.stompBounceForce);
    }

    private void EndStaggerWithoutStomp()
    {
        SetHint(false);
        ctx.State = BossBehaviorState.Active;
        SetBodySolid(false);
        ctx.IsRunAnimPlaying = false;
        ResetFireTimer();
        ctx.PhaseEndTime = Time.time + GetPhaseDuration(ctx.CurrentPhase);
        movement.ResumePatrol();
    }

    private void PlayDieRecoverAnimation()
    {
        ctx.RecoveryStep = BossRecoveryStep.DieRecover;
        ctx.RecoveryEndTime = Time.time + boss.dieRecoverAnimDuration;
        animation.PlayDieRecover();
    }

    private void FinishRecovery()
    {
        if (ctx.CurrentPhase < 3)
        {
            ctx.CurrentPhase++;
            ApplySettings();
            ctx.PhaseEndTime = Time.time + GetPhaseDuration(ctx.CurrentPhase);
        }

        ctx.State = BossBehaviorState.Active;
        SetBodySolid(false);
        ctx.IsRunAnimPlaying = false;
        movement.ResumePatrol();
    }

    private void BeginDeathSequence()
    {
        ctx.State = BossBehaviorState.Dying;
        SetBodySolid(false);
        ctx.RecoveryEndTime = Time.time + boss.dieAnimDuration;
        movement.Stop();
        animation.CancelAttack();
        animation.PlayDie();
    }

    private float GetPhaseDuration(int phase) =>
        phase == 1 ? boss.phase1Duration : phase == 2 ? boss.phase2Duration : boss.phase3Duration;

    private void SetBodySolid(bool solid)
    {
        if (ctx.BodyCollider == null) return;
        ctx.BodyCollider.isTrigger = solid ? false : ctx.BodyColliderWasTrigger;
    }

    private void SetHint(bool visible)
    {
        if (BossHintUI.Instance == null) return;
        if (visible) BossHintUI.Instance.ShowStompHint();
        else BossHintUI.Instance.Hide();
    }
}
