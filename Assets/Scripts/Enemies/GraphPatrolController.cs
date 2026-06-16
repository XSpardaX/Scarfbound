using UnityEngine;
using UnityEngine.AI;

public class GraphPatrolController : MonoBehaviour
{
    public PatrolGraphBuilder patrolGraphBuilder;
    public PatrolBranchStrategy branchStrategy = PatrolBranchStrategy.Random;
    public float reachDistance = 0.5f;

    private NavMeshAgent agent;
    private PatrolGraph patrolGraph;

    private int currentNodeId;
    private bool isWaitingAtNode;
    private float nodeWaitTimer;
    private bool patrolEnabled = true;

    public PatrolGraph Graph => patrolGraph;

    public void Initialize()
    {
        agent = GetComponent<NavMeshAgent>();

        if (patrolGraphBuilder == null)
        {
            patrolGraphBuilder = GetComponentInChildren<PatrolGraphBuilder>();
        }

        if (patrolGraphBuilder == null)
        {
            Debug.LogError($"[{name}] GraphPatrolController requires a PatrolGraphBuilder.");
            return;
        }

        patrolGraph = patrolGraphBuilder.BuildGraph();
        BeginPatrol();
    }

    public void SetPatrolEnabled(bool enabled)
    {
        patrolEnabled = enabled;

        if (enabled)
        {
            isWaitingAtNode = false;
        }
    }

    public void TickPatrol()
    {
        if (!patrolEnabled || patrolGraph == null || patrolGraph.NodeCount == 0) return;

        if (isWaitingAtNode)
        {
            nodeWaitTimer -= Time.deltaTime;
            if (nodeWaitTimer > 0f) return;

            isWaitingAtNode = false;
            MoveToNextPatrolNode();
            return;
        }

        if (agent.pathPending) return;
        if (agent.remainingDistance > reachDistance) return;

        float waitTime = patrolGraph.GetWaitTime(currentNodeId);
        if (waitTime > 0f)
        {
            isWaitingAtNode = true;
            nodeWaitTimer = waitTime;
            return;
        }

        MoveToNextPatrolNode();
    }

    public void ResumeFromCurrentPosition()
    {
        if (patrolGraph == null || patrolGraph.NodeCount == 0) return;

        isWaitingAtNode = false;
        currentNodeId = patrolGraph.FindNearestNodeId(transform.position);
        agent.SetDestination(patrolGraph.GetPosition(currentNodeId));
    }

    private void BeginPatrol()
    {
        currentNodeId = patrolGraph.StartNodeId;
        isWaitingAtNode = false;

        Vector3 startPosition = patrolGraph.GetPosition(currentNodeId);
        if (NavMesh.SamplePosition(startPosition, out NavMeshHit hit, 2f, NavMesh.AllAreas))
        {
            agent.Warp(hit.position);
            startPosition = hit.position;
        }

        agent.SetDestination(startPosition);
    }

    private void MoveToNextPatrolNode()
    {
        int nextNodeId = patrolGraph.ChooseNextNode(currentNodeId, branchStrategy);
        currentNodeId = nextNodeId;
        agent.SetDestination(patrolGraph.GetPosition(currentNodeId));
    }
}
