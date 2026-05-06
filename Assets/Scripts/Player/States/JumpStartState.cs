using UnityEngine;

public class JumpStartState : PlayerState
{
    private const float BlendDuration = 0.05f;

    public JumpStartState(Player player, PlayerStateMachine sm, Animator animator) : base(player, sm, animator) 
    {
    
    }

    public override void Enter()
    {
        animator.CrossFadeInFixedTime("JumpStart", BlendDuration);
    }

    public override void Tick()
    {
        if (player.VerticalVelocity <= 0f)
        {
            stateMachine.ChangeState(player.Falling);
        }
    }
}
