using UnityEngine;

public abstract class PlayerState
{
    protected readonly Player player;
    protected readonly PlayerStateMachine stateMachine;
    protected readonly Animator animator;

    protected PlayerState(Player player, PlayerStateMachine stateMachine, Animator animator)
    {
        this.player = player;
        this.stateMachine = stateMachine;
        this.animator = animator;
    }

    public virtual void Enter() { }
    public virtual void Exit() { }
    public virtual void Tick() { }

    protected PlayerState GetGroundedStateFromInput()
    {
        if (player.IsMovementBlocked || !player.IsMoving)
        {
            return player.Idle;
        }

        return player.Run;
    }

    protected bool TryTransitionToAir()
    {
        if (player.IsGrounded) return false;

        if (player.VerticalVelocity > 0f)
        {
            stateMachine.ChangeState(player.JumpStart);
        }
        else
        {
            stateMachine.ChangeState(player.Falling);
        }

        return true;
    }
}
