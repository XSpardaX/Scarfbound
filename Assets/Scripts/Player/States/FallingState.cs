using UnityEngine;

public class FallingState : PlayerState
{
    private const float BlendDuration = 0.15f;

    public FallingState(Player player, PlayerStateMachine sm, Animator animator) : base(player, sm, animator) 
    { 
    
    }

    public override void Enter()
    {
        animator.CrossFadeInFixedTime("Falling", BlendDuration);
    }

    public override void Tick()
    {
        if (player.IsGrounded)
        {
            stateMachine.ChangeState(GetGroundedStateFromInput());
        }
    }
}
