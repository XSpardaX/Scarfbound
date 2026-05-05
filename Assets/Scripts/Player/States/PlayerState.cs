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

    // Any movement input plays Run; no input goes Idle.
    protected PlayerState GetGroundedStateFromInput()
    {
        Vector2 input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        return input.sqrMagnitude < 0.01f ? (PlayerState)player.Idle : player.Run;
    }

    // Common air-check used by every grounded state.
    protected bool TryTransitionToAir()
    {
        if (player.IsGrounded) return false;

        stateMachine.ChangeState(player.VerticalVelocity > 0f
            ? (PlayerState)player.JumpStart
            : player.Falling);
        return true;
    }
}
