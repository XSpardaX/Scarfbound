using UnityEngine;

public class BossPhaseController
{
    private readonly BossContext bossContext;
    private readonly BossEnemy bossSettings;
    private readonly BossMovementController movementController;
    private readonly BossAnimationController animationController;

    public BossPhaseController(
        BossContext context,
        BossEnemy bossEnemy,
        BossMovementController bossMovementController,
        BossAnimationController bossAnimationController)
    {
        bossContext = context;
        bossSettings = bossEnemy;
        movementController = bossMovementController;
        animationController = bossAnimationController;
    }

    public void ApplySettings()
    {
        if (bossContext.CurrentPhase == 1)
        {
            bossContext.Agent.speed = bossSettings.phase1MoveSpeed;
            bossContext.CurrentProjectileSpeed = bossSettings.phase1ProjectileSpeed;
            bossContext.CurrentFireIntervalMultiplier = bossSettings.phase1FireIntervalMultiplier;

            if (bossContext.FightStarted)
            {
                bossContext.Agent.updateRotation = false;
            }
            else
            {
                bossContext.Agent.updateRotation = true;
            }

            return;
        }

        if (bossContext.CurrentPhase == 2)
        {
            bossContext.Agent.speed = bossSettings.phase2MoveSpeed;
            bossContext.CurrentProjectileSpeed = bossSettings.phase2ProjectileSpeed;
            bossContext.CurrentFireIntervalMultiplier = bossSettings.phase2FireIntervalMultiplier;
            bossContext.Agent.updateRotation = false;
            return;
        }

        bossContext.Agent.speed = bossSettings.phase3MoveSpeed;
        bossContext.CurrentProjectileSpeed = bossSettings.phase3ProjectileSpeed;
        bossContext.CurrentFireIntervalMultiplier = bossSettings.phase3FireIntervalMultiplier;
        bossContext.Agent.updateRotation = false;
        bossContext.Phase3AlternateShot = false;
    }

    public void StartFight()
    {
        if (bossContext.FightStarted)
        {
            return;
        }

        bossContext.FightStarted = true;
        bossContext.State = BossBehaviorState.Active;
        bossContext.CurrentPhase = 1;
        bossContext.PhaseEndTime = Time.time + bossSettings.phase1Duration;
        bossContext.IsPhaseTimerPaused = false;
        bossContext.IsRunAnimPlaying = false;

        ResetFireTimer();
        ApplySettings();

        bossContext.Agent.isStopped = false;
        bossContext.Patrol.ResumeFromCurrentPosition();
    }

    public void ResetEncounter()
    {
        bossContext.FightStarted = false;
        bossContext.State = BossBehaviorState.Active;
        bossContext.RecoveryStep = BossRecoveryStep.GetHit;

        bossContext.IsRunAnimPlaying = false;
        bossContext.IsAttacking = false;
        bossContext.IsDeathStomp = false;
        bossContext.IsPhaseTimerPaused = false;

        bossContext.StompCount = 0;
        bossContext.CurrentPhase = 1;
        bossContext.Phase3AlternateShot = false;

        ResetFireTimer();
        SetBodySolid(false);

        bossContext.Boss.transform.SetPositionAndRotation(
            bossContext.SpawnPosition,
            bossContext.SpawnRotation);

        ApplySettings();
        movementController.Stop();
        animationController.PlayIdle();
    }

    public void TickTimedStates()
    {
        if (bossContext.State == BossBehaviorState.Recovering)
        {
            if (Time.time < bossContext.RecoveryEndTime)
            {
                return;
            }

            if (bossContext.RecoveryStep == BossRecoveryStep.GetHit)
            {
                if (bossContext.IsDeathStomp)
                {
                    BeginDeathSequence();
                }
                else
                {
                    PlayDieRecoverAnimation();
                }
            }
            else
            {
                FinishRecovery();
            }

            return;
        }

        if (bossContext.State == BossBehaviorState.Dying && Time.time >= bossContext.RecoveryEndTime)
        {
            if (bossSettings.keyPrefab != null)
            {
                Object.Instantiate(
                    bossSettings.keyPrefab,
                    bossContext.Boss.transform.position + bossSettings.keySpawnOffset,
                    Quaternion.identity);
            }

            Object.Destroy(bossContext.Boss.gameObject);
        }
    }

    public void TickStagger()
    {
        if (bossContext.State != BossBehaviorState.Staggered)
        {
            return;
        }

        if (Time.time >= bossContext.StaggerEndTime)
        {
            EndStaggerWithoutStomp();
        }
    }

    public void TickPhaseTimer()
    {
        if (bossContext.State != BossBehaviorState.Active)
        {
            return;
        }

        if (Time.time >= bossContext.PhaseEndTime)
        {
            EnterStagger();
        }
    }

    public void ResetFireTimer()
    {
        bossContext.FireTimer = 0f;
    }

    public void EnterStagger()
    {
        if (bossContext.State == BossBehaviorState.Staggered)
        {
            return;
        }

        bossContext.State = BossBehaviorState.Staggered;
        SetBodySolid(true);
        ResetFireTimer();

        bossContext.StaggerEndTime = Time.time + bossSettings.staggerDuration;

        animationController.CancelAttack();
        movementController.Stop();
        bossContext.Agent.updateRotation = true;
        animationController.PlayDizzy();
    }

    public void BeginHeadStompRecovery(Player touchingPlayer)
    {
        bossContext.StompCount += 1;
        bossContext.IsDeathStomp = bossContext.StompCount >= 3;
        bossContext.State = BossBehaviorState.Recovering;
        bossContext.RecoveryStep = BossRecoveryStep.GetHit;
        bossContext.RecoveryEndTime = Time.time + bossSettings.getHitAnimDuration;

        SetBodySolid(true);
        movementController.Stop();
        bossContext.Agent.updateRotation = true;
        animationController.PlayGetHit();

        if (SfxManager.Instance != null)
        {
            SfxManager.Instance.PlayBossHurt();
        }

        touchingPlayer.ApplyBounce(bossSettings.stompBounceForce);
    }

    private void EndStaggerWithoutStomp()
    {
        bossContext.State = BossBehaviorState.Active;
        bossContext.IsRunAnimPlaying = false;

        SetBodySolid(false);
        ResetFireTimer();
        bossContext.PhaseEndTime = Time.time + GetPhaseDuration(bossContext.CurrentPhase);

        movementController.ResumePatrol();
    }

    private void PlayDieRecoverAnimation()
    {
        bossContext.RecoveryStep = BossRecoveryStep.DieRecover;
        bossContext.RecoveryEndTime = Time.time + bossSettings.dieRecoverAnimDuration;
        animationController.PlayDieRecover();
    }

    private void FinishRecovery()
    {
        if (bossContext.CurrentPhase < 3)
        {
            bossContext.CurrentPhase += 1;
            ApplySettings();
            bossContext.PhaseEndTime = Time.time + GetPhaseDuration(bossContext.CurrentPhase);
        }

        bossContext.State = BossBehaviorState.Active;
        bossContext.IsRunAnimPlaying = false;
        SetBodySolid(false);
        movementController.ResumePatrol();
    }

    private void BeginDeathSequence()
    {
        bossContext.State = BossBehaviorState.Dying;
        bossContext.RecoveryEndTime = Time.time + bossSettings.dieAnimDuration;

        SetBodySolid(false);
        movementController.Stop();
        animationController.CancelAttack();
        animationController.PlayDie();
    }

    private float GetPhaseDuration(int phaseNumber)
    {
        if (phaseNumber == 1)
        {
            return bossSettings.phase1Duration;
        }

        if (phaseNumber == 2)
        {
            return bossSettings.phase2Duration;
        }

        return bossSettings.phase3Duration;
    }

    private void SetBodySolid(bool shouldBeSolid)
    {
        if (bossContext.BodyCollider == null)
        {
            return;
        }

        if (shouldBeSolid)
        {
            bossContext.BodyCollider.isTrigger = false;
        }
        else
        {
            bossContext.BodyCollider.isTrigger = bossContext.BodyColliderWasTrigger;
        }
    }
}
