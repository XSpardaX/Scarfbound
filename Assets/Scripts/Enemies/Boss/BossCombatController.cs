using UnityEngine;

public class BossCombatController
{
    private readonly BossContext bossContext;
    private readonly BossEnemy bossSettings;
    private readonly BossMovementController movementController;
    private readonly BossAnimationController animationController;
    private readonly BossPhaseController phaseController;

    public BossCombatController(
        BossContext context,
        BossEnemy bossEnemy,
        BossMovementController bossMovementController,
        BossAnimationController bossAnimationController,
        BossPhaseController bossPhaseController)
    {
        bossContext = context;
        bossSettings = bossEnemy;
        movementController = bossMovementController;
        animationController = bossAnimationController;
        phaseController = bossPhaseController;
    }

    public void TickDefend()
    {
        float distanceToPlayer = movementController.GetHorizontalDistanceToPlayer();

        if (bossContext.State == BossBehaviorState.Defending)
        {
            if (distanceToPlayer > bossSettings.defendReleaseDistance)
            {
                ExitDefend();
            }

            return;
        }

        if (distanceToPlayer <= bossSettings.defendDistance)
        {
            EnterDefend();
        }
    }

    public void OnPlayerContact(Player touchingPlayer)
    {
        if (!bossContext.FightStarted)
        {
            return;
        }

        bool stompStarted = TryStomp(touchingPlayer);
        if (stompStarted)
        {
            return;
        }

        if (bossContext.IsHarmless)
        {
            return;
        }

        PlayerHealth playerHealth = touchingPlayer.GetComponent<PlayerHealth>();
        if (playerHealth != null && !playerHealth.IsInvincible)
        {
            playerHealth.TakeDamage(2);
        }
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (!bossContext.FightStarted)
        {
            return;
        }

        if (bossContext.IsHarmless)
        {
            return;
        }

        if (!collision.gameObject.CompareTag("Player"))
        {
            return;
        }

        Rigidbody playerRigidbody = collision.rigidbody;
        if (playerRigidbody == null)
        {
            return;
        }

        Vector3 knockbackDirection = collision.transform.position - bossContext.Boss.transform.position;
        knockbackDirection = knockbackDirection.normalized;
        playerRigidbody.AddForce(knockbackDirection * bossSettings.knockbackForce, ForceMode.Impulse);
    }

    public void OnCollisionStay(Collision collision)
    {
        if (!bossContext.FightStarted)
        {
            return;
        }

        if (!collision.gameObject.CompareTag("Player"))
        {
            return;
        }

        Player touchingPlayer = collision.gameObject.GetComponentInParent<Player>();
        if (touchingPlayer != null)
        {
            TryStomp(touchingPlayer);
        }
    }

    private void EnterDefend()
    {
        if (bossContext.State == BossBehaviorState.Defending)
        {
            return;
        }

        bossContext.State = BossBehaviorState.Defending;
        bossContext.IsRunAnimPlaying = false;

        phaseController.ResetFireTimer();
        animationController.CancelAttack();
        movementController.Stop();
        bossContext.Agent.updateRotation = false;

        if (bossContext.FightStarted && !bossContext.IsPhaseTimerPaused)
        {
            bossContext.IsPhaseTimerPaused = true;
            bossContext.PhasePauseStartTime = Time.time;
        }

        animationController.PlayDefend();
    }

    private void ExitDefend()
    {
        if (bossContext.State != BossBehaviorState.Defending)
        {
            return;
        }

        bossContext.State = BossBehaviorState.Active;
        bossContext.IsRunAnimPlaying = false;

        if (bossContext.IsPhaseTimerPaused)
        {
            float pausedDuration = Time.time - bossContext.PhasePauseStartTime;
            bossContext.PhaseEndTime += pausedDuration;
            bossContext.IsPhaseTimerPaused = false;
        }

        movementController.ResumePatrol();
    }

    private bool TryStomp(Player touchingPlayer)
    {
        if (bossContext.State != BossBehaviorState.Staggered)
        {
            return false;
        }

        if (touchingPlayer.VerticalVelocity >= 0f)
        {
            return false;
        }

        Vector3 bossHeadPosition;
        if (bossSettings.headPoint != null)
        {
            bossHeadPosition = bossSettings.headPoint.position;
        }
        else
        {
            bossHeadPosition = bossContext.Boss.transform.position + Vector3.up * bossSettings.headHeightOffset;
        }

        if (touchingPlayer.transform.position.y <= bossHeadPosition.y)
        {
            return false;
        }

        Vector3 playerHorizontalOffset = touchingPlayer.transform.position - bossHeadPosition;
        playerHorizontalOffset.y = 0f;

        float stompRadiusSquared = bossSettings.headStompRadius * bossSettings.headStompRadius;
        if (playerHorizontalOffset.sqrMagnitude > stompRadiusSquared)
        {
            return false;
        }

        phaseController.BeginHeadStompRecovery(touchingPlayer);
        return true;
    }
}
