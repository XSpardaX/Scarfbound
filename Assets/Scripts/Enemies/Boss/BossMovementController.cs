using UnityEngine;

// patrol path and look at player
public class BossMovementController
{
    private readonly BossContext bossContext;

    public BossMovementController(BossContext context)
    {
        bossContext = context;
    }

    // stop and resume walking
    public void Stop()
    {
        bossContext.Patrol.SetPatrolEnabled(false);

        bossContext.Agent.isStopped = true;
        bossContext.Agent.ResetPath();
        bossContext.Agent.velocity = Vector3.zero;
        bossContext.Agent.nextPosition = bossContext.Boss.transform.position;
        bossContext.Agent.Warp(bossContext.Boss.transform.position);
    }

    public void KeepStopped()
    {
        if (!bossContext.Agent.isStopped)
        {
            bossContext.Agent.isStopped = true;
        }

        if (bossContext.Agent.hasPath)
        {
            bossContext.Agent.ResetPath();
        }

        if (bossContext.Agent.velocity.sqrMagnitude > 0.0001f)
        {
            bossContext.Agent.velocity = Vector3.zero;
            bossContext.Agent.nextPosition = bossContext.Boss.transform.position;
        }
    }

    public void ResumePatrol()
    {
        if (bossContext.FightStarted)
        {
            bossContext.Agent.updateRotation = false;
        }
        else
        {
            bossContext.Agent.updateRotation = true;
        }

        bossContext.Agent.isStopped = false;
        bossContext.Patrol.ResumeFromCurrentPosition();
    }

    public void TickPatrol()
    {
        bossContext.Patrol.SetPatrolEnabled(true);
        bossContext.Patrol.TickPatrol();
    }

    // how far and turn toward player
    public float GetHorizontalDistanceToPlayer()
    {
        if (bossContext.Player == null)
        {
            return float.MaxValue;
        }

        Vector3 playerOffsetFromBoss = bossContext.Player.position - bossContext.Boss.transform.position;
        playerOffsetFromBoss.y = 0f;

        float horizontalDistance = playerOffsetFromBoss.magnitude;
        return horizontalDistance;
    }

    public void FacePlayer(float turnSpeed)
    {
        if (bossContext.Player == null)
        {
            return;
        }

        Vector3 directionToPlayer = bossContext.Player.position - bossContext.Boss.transform.position;
        directionToPlayer.y = 0f;

        if (directionToPlayer.sqrMagnitude < 0.0001f)
        {
            return;
        }

        Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer.normalized);
        bossContext.Boss.transform.rotation = Quaternion.Slerp(
            bossContext.Boss.transform.rotation,
            targetRotation,
            turnSpeed * Time.deltaTime);
    }
}
