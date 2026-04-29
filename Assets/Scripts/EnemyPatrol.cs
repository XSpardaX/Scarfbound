using UnityEngine;
using UnityEngine.AI;

public class EnemyPatrol : MonoBehaviour
{
    public Waypoint startNode;
    public float reachDistance = 0.3f;

    private Waypoint currentNode;
    private NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        currentNode = startNode;

        if (currentNode != null)
        {
            agent.SetDestination(currentNode.transform.position);
        }
    }

    void Update()
    {
        if (currentNode == null) return;

        // Check if agent reached current node
        if (!agent.pathPending && agent.remainingDistance <= reachDistance)
        {
            if (currentNode.nextNode != null)
            {
                currentNode = currentNode.nextNode;
                agent.SetDestination(currentNode.transform.position);
            }
        }
    }
}