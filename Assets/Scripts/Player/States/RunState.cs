using UnityEngine;

public class RunState : PlayerState
{
    private const float BlendDuration = 0.12f;

    public RunState(Player player, PlayerStateMachine sm, Animator animator) : base(player, sm, animator) 
    { 

    }

    public override void Enter()
    {
        if (player.IsMovementBlocked || !player.IsMoving)
        {
            stateMachine.ChangeState(player.Idle);
            return;
        }

        animator.CrossFadeInFixedTime("Run", BlendDuration);
    }

    public override void Tick()
    {
        if (player.IsMovementBlocked)
        {
            stateMachine.ChangeState(player.Idle);
            return;
        }

        if (TryTransitionToAir()) return;
        stateMachine.ChangeState(GetGroundedStateFromInput());
    }
}
