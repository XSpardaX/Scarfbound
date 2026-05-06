using UnityEngine;

public class IdleState : PlayerState
{
    private const float BlendDuration = 0.35f;

    public IdleState(Player player, PlayerStateMachine sm, Animator animator) : base(player, sm, animator) 
    {
    
    }

    public override void Enter()
    {
        animator.CrossFadeInFixedTime("Idle", BlendDuration);
    }

    public override void Tick()
    {
        if (TryTransitionToAir()) return;
        stateMachine.ChangeState(GetGroundedStateFromInput());
    }
}
