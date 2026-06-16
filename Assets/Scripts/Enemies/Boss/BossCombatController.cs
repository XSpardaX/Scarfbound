using UnityEngine;

public class BossCombatController
{
    private readonly BossContext ctx;
    private readonly BossEnemy boss;
    private readonly BossMovementController movement;
    private readonly BossAnimationController animation;
    private readonly BossPhaseController phases;

    public BossCombatController(
        BossContext context,
        BossEnemy bossEnemy,
        BossMovementController movementController,
        BossAnimationController animationController,
        BossPhaseController phaseController)
    {
        ctx = context;
        boss = bossEnemy;
        movement = movementController;
        animation = animationController;
        phases = phaseController;
    }

    public void TickDefend()
    {
        float distance = movement.GetHorizontalDistanceToPlayer();

        if (ctx.State == BossBehaviorState.Defending)
        {
            if (distance > boss.defendReleaseDistance) ExitDefend();
            return;
        }

        if (distance <= boss.defendDistance) EnterDefend();
    }

    public void OnPlayerContact(Player touchingPlayer)
    {
        if (!ctx.FightStarted || TryStomp(touchingPlayer) || ctx.IsHarmless) return;

        PlayerHealth playerHealth = touchingPlayer.GetComponent<PlayerHealth>();
        if (playerHealth != null && !playerHealth.IsInvincible) playerHealth.TakeDamage(2);
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (!ctx.FightStarted || ctx.IsHarmless || !collision.gameObject.CompareTag("Player")) return;

        Rigidbody playerRigidbody = collision.rigidbody;
        if (playerRigidbody == null) return;

        Vector3 knockbackDirection = (collision.transform.position - ctx.Boss.transform.position).normalized;
        playerRigidbody.AddForce(knockbackDirection * boss.knockbackForce, ForceMode.Impulse);
    }

    public void OnCollisionStay(Collision collision)
    {
        if (!ctx.FightStarted || !collision.gameObject.CompareTag("Player")) return;
        Player touchingPlayer = collision.gameObject.GetComponentInParent<Player>();
        if (touchingPlayer != null) TryStomp(touchingPlayer);
    }

    private void EnterDefend()
    {
        if (ctx.State == BossBehaviorState.Defending) return;

        ctx.State = BossBehaviorState.Defending;
        ctx.IsRunAnimPlaying = false;
        phases.ResetFireTimer();
        animation.CancelAttack();
        movement.Stop();
        ctx.Agent.updateRotation = false;

        if (ctx.FightStarted && !ctx.IsPhaseTimerPaused)
        {
            ctx.IsPhaseTimerPaused = true;
            ctx.PhasePauseStartTime = Time.time;
        }

        animation.PlayDefend();
    }

    private void ExitDefend()
    {
        if (ctx.State != BossBehaviorState.Defending) return;

        ctx.State = BossBehaviorState.Active;
        ctx.IsRunAnimPlaying = false;

        if (ctx.IsPhaseTimerPaused)
        {
            ctx.PhaseEndTime += Time.time - ctx.PhasePauseStartTime;
            ctx.IsPhaseTimerPaused = false;
        }

        movement.ResumePatrol();
    }

    private bool TryStomp(Player touchingPlayer)
    {
        if (ctx.State != BossBehaviorState.Staggered || touchingPlayer.VerticalVelocity >= 0f) return false;

        Vector3 headPosition = boss.headPoint != null
            ? boss.headPoint.position
            : ctx.Boss.transform.position + Vector3.up * boss.headHeightOffset;

        if (touchingPlayer.transform.position.y <= headPosition.y) return false;

        Vector3 horizontalOffset = touchingPlayer.transform.position - headPosition;
        horizontalOffset.y = 0f;
        if (horizontalOffset.sqrMagnitude > boss.headStompRadius * boss.headStompRadius) return false;

        phases.BeginHeadStompRecovery(touchingPlayer);
        return true;
    }
}
