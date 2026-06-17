using UnityEngine;

// boss attack, walk, and hurt anims
public class BossAnimationController
{
    private readonly BossContext bossContext;
    private readonly BossEnemy bossSettings;

    public BossAnimationController(BossContext context, BossEnemy bossEnemy)
    {
        bossContext = context;
        bossSettings = bossEnemy;
    }

    // play anim clips
    public void Initialize()
    {
        if (bossContext.Animator == null)
        {
            return;
        }

        bossContext.Animator.SetLayerWeight(bossSettings.upperBodyLayerIndex, 0f);
    }

    public void CrossFade(string animationStateName, float blendDuration = -1f, int animationLayer = 0)
    {
        if (bossContext.Animator == null)
        {
            return;
        }

        float finalBlendDuration = bossSettings.animBlendDuration;
        if (blendDuration >= 0f)
        {
            finalBlendDuration = blendDuration;
        }

        bossContext.Animator.CrossFadeInFixedTime(animationStateName, finalBlendDuration, animationLayer);
    }

    public void CancelAttack()
    {
        bossContext.IsAttacking = false;

        if (bossContext.Animator != null)
        {
            bossContext.Animator.SetLayerWeight(bossSettings.upperBodyLayerIndex, 0f);
        }
    }

    public void PlayAttack()
    {
        if (bossContext.Animator == null)
        {
            return;
        }

        bossContext.IsAttacking = true;
        bossContext.AttackEndTime = Time.time + bossSettings.attackAnimDuration;

        bossContext.Animator.SetLayerWeight(bossSettings.upperBodyLayerIndex, 1f);
        CrossFade(bossSettings.attackAnimState, 0.05f, bossSettings.upperBodyLayerIndex);
    }

    public void PlayIntroAttack()
    {
        if (bossContext.Animator == null)
        {
            return;
        }

        if (string.IsNullOrEmpty(bossSettings.introAttackAnimState))
        {
            return;
        }

        bossContext.Animator.SetLayerWeight(bossSettings.upperBodyLayerIndex, 0f);

        int introAnimationHash = Animator.StringToHash(bossSettings.introAttackAnimState);
        bool introAnimationExists = bossContext.Animator.HasState(bossSettings.introAttackLayerIndex, introAnimationHash);

        string animationStateToPlay;
        int layerToPlay;

        if (introAnimationExists)
        {
            animationStateToPlay = bossSettings.introAttackAnimState;
            layerToPlay = bossSettings.introAttackLayerIndex;
        }
        else
        {
            animationStateToPlay = bossSettings.attackAnimState;
            layerToPlay = bossSettings.upperBodyLayerIndex;
        }

        CrossFade(animationStateToPlay, 0.05f, layerToPlay);
    }

    // idle and run anims
    public void UpdateLocomotion()
    {
        if (bossContext.Animator == null)
        {
            return;
        }

        float movementThresholdSquared = bossSettings.moveAnimThreshold * bossSettings.moveAnimThreshold;
        bool shouldPlayRunAnimation = bossContext.Agent.velocity.sqrMagnitude > movementThresholdSquared;

        if (shouldPlayRunAnimation == bossContext.IsRunAnimPlaying)
        {
            return;
        }

        bossContext.IsRunAnimPlaying = shouldPlayRunAnimation;

        if (shouldPlayRunAnimation)
        {
            CrossFade(bossSettings.runAnimState);
        }
        else
        {
            CrossFade(bossSettings.idleAnimState);
        }
    }

    // defend, dizzy, hit, die anims
    public void PlayDefend()
    {
        CrossFade(bossSettings.defendAnimState);
    }

    public void PlayDizzy()
    {
        CrossFade(bossSettings.dizzyAnimState);
    }

    public void PlayGetHit()
    {
        if (bossContext.Animator != null)
        {
            bossContext.Animator.SetLayerWeight(bossSettings.upperBodyLayerIndex, 0f);
        }

        CrossFade(bossSettings.getHitAnimState, 0.05f);
    }

    public void PlayDieRecover()
    {
        CrossFade(bossSettings.dieRecoverAnimState, 0.05f);
    }

    public void PlayDie()
    {
        if (bossContext.Animator != null)
        {
            bossContext.Animator.SetLayerWeight(bossSettings.upperBodyLayerIndex, 0f);
        }

        CrossFade(bossSettings.dieAnimState, 0.05f);
    }

    public void PlayIdle()
    {
        if (bossContext.Animator != null)
        {
            bossContext.Animator.SetLayerWeight(bossSettings.upperBodyLayerIndex, 0f);
        }

        CrossFade(bossSettings.idleAnimState);
    }
}
