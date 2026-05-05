using UnityEngine;

public class RunState : PlayerState
{
    private const float BlendDuration = 0.12f;

    public RunState(Player player, PlayerStateMachine sm, Animator animator) : base(player, sm, animator) 
    { 
    }

    public override void Enter()
    {
        animator.CrossFadeInFixedTime("Run", BlendDuration);
    }

    public override void Tick()
    {
        if (TryTransitionToAir()) return;
        stateMachine.ChangeState(GetGroundedStateFromInput());
    }
}
