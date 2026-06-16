public class BossAnimationController
{
    private readonly BossContext ctx;
    private readonly BossEnemy boss;

    public BossAnimationController(BossContext context, BossEnemy bossEnemy)
    {
        ctx = context;
        boss = bossEnemy;
    }

    public void Initialize()
    {
        if (ctx.Animator != null) ctx.Animator.SetLayerWeight(boss.upperBodyLayerIndex, 0f);
    }

    public void CrossFade(string state, float blend = -1f, int layer = 0)
    {
        if (ctx.Animator == null) return;
        float duration = blend < 0f ? boss.animBlendDuration : blend;
        ctx.Animator.CrossFadeInFixedTime(state, duration, layer);
    }

    public void CancelAttack()
    {
        ctx.IsAttacking = false;
        if (ctx.Animator != null) ctx.Animator.SetLayerWeight(boss.upperBodyLayerIndex, 0f);
    }

    public void PlayAttack()
    {
        if (ctx.Animator == null) return;

        ctx.IsAttacking = true;
        ctx.AttackEndTime = UnityEngine.Time.time + boss.attackAnimDuration;
        ctx.Animator.SetLayerWeight(boss.upperBodyLayerIndex, 1f);
        CrossFade(boss.attackAnimState, 0.05f, boss.upperBodyLayerIndex);
    }

    public void PlayIntroAttack()
    {
        if (ctx.Animator == null || string.IsNullOrEmpty(boss.introAttackAnimState)) return;

        ctx.Animator.SetLayerWeight(boss.upperBodyLayerIndex, 0f);
        string state = ctx.Animator.HasState(boss.introAttackLayerIndex, UnityEngine.Animator.StringToHash(boss.introAttackAnimState))
            ? boss.introAttackAnimState
            : boss.attackAnimState;
        int layer = state == boss.introAttackAnimState ? boss.introAttackLayerIndex : boss.upperBodyLayerIndex;
        CrossFade(state, 0.05f, layer);
    }

    public void UpdateLocomotion()
    {
        if (ctx.Animator == null) return;

        bool shouldRun = ctx.Agent.velocity.sqrMagnitude > boss.moveAnimThreshold * boss.moveAnimThreshold;
        if (shouldRun == ctx.IsRunAnimPlaying) return;

        ctx.IsRunAnimPlaying = shouldRun;
        CrossFade(shouldRun ? boss.runAnimState : boss.idleAnimState);
    }

    public void PlayDefend() => CrossFade(boss.defendAnimState);
    public void PlayDizzy() => CrossFade(boss.dizzyAnimState);
    public void PlayGetHit()
    {
        if (ctx.Animator != null) ctx.Animator.SetLayerWeight(boss.upperBodyLayerIndex, 0f);
        CrossFade(boss.getHitAnimState, 0.05f);
    }

    public void PlayDieRecover() => CrossFade(boss.dieRecoverAnimState, 0.05f);
    public void PlayDie()
    {
        if (ctx.Animator != null) ctx.Animator.SetLayerWeight(boss.upperBodyLayerIndex, 0f);
        CrossFade(boss.dieAnimState, 0.05f);
    }

    public void PlayIdle()
    {
        if (ctx.Animator != null) ctx.Animator.SetLayerWeight(boss.upperBodyLayerIndex, 0f);
        CrossFade(boss.idleAnimState);
    }
}
