public class BossMovementController
{
    private readonly BossContext ctx;

    public BossMovementController(BossContext context)
    {
        ctx = context;
    }

    public void Stop()
    {
        ctx.Patrol.SetPatrolEnabled(false);
        ctx.Agent.isStopped = true;
        ctx.Agent.ResetPath();
        ctx.Agent.velocity = UnityEngine.Vector3.zero;
        ctx.Agent.nextPosition = ctx.Boss.transform.position;
        ctx.Agent.Warp(ctx.Boss.transform.position);
    }

    public void KeepStopped()
    {
        if (!ctx.Agent.isStopped) ctx.Agent.isStopped = true;
        if (ctx.Agent.hasPath) ctx.Agent.ResetPath();
        if (ctx.Agent.velocity.sqrMagnitude > 0.0001f)
        {
            ctx.Agent.velocity = UnityEngine.Vector3.zero;
            ctx.Agent.nextPosition = ctx.Boss.transform.position;
        }
    }

    public void ResumePatrol()
    {
        ctx.Agent.updateRotation = !ctx.FightStarted;
        ctx.Agent.isStopped = false;
        ctx.Patrol.ResumeFromCurrentPosition();
    }

    public void TickPatrol()
    {
        ctx.Patrol.SetPatrolEnabled(true);
        ctx.Patrol.TickPatrol();
    }

    public float GetHorizontalDistanceToPlayer()
    {
        if (ctx.Player == null) return float.MaxValue;

        UnityEngine.Vector3 offset = ctx.Player.position - ctx.Boss.transform.position;
        offset.y = 0f;
        return offset.magnitude;
    }

    public void FacePlayer(float turnSpeed)
    {
        if (ctx.Player == null) return;

        UnityEngine.Vector3 direction = ctx.Player.position - ctx.Boss.transform.position;
        direction.y = 0f;
        if (direction.sqrMagnitude < 0.0001f) return;

        ctx.Boss.transform.rotation = UnityEngine.Quaternion.Slerp(
            ctx.Boss.transform.rotation,
            UnityEngine.Quaternion.LookRotation(direction.normalized),
            turnSpeed * UnityEngine.Time.deltaTime);
    }
}
