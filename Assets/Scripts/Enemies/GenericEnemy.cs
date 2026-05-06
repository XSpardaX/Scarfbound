using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GenericEnemy : EnemyBase
{
    public Waypoint startNode;
    public float reachDistance = 0.3f;

    public Transform eyePoint;
    public float viewDistance = 8f;
    public float viewAngle = 120f;
    public LayerMask obstacleMask;

    public float knockbackForce = 6f;

    private NavMeshAgent agent;
    private Transform player;
    private bool chasing;

    private Vector3[] patrolPath;
    private int patrolIndex;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
        }

        BuildPatrolPath();

        if (patrolPath.Length > 0)
        {
            agent.SetDestination(patrolPath[0]);
        }
    }

    private void BuildPatrolPath()
    {
        List<Waypoint> patrolNodes = new List<Waypoint>();
        HashSet<Waypoint> visitedNodes = new HashSet<Waypoint>();

        Waypoint currentNode = startNode;
        while (currentNode != null && visitedNodes.Add(currentNode))
        {
            patrolNodes.Add(currentNode);
            currentNode = currentNode.nextNode;
        }

        patrolPath = new Vector3[patrolNodes.Count];
        for (int i = 0; i < patrolNodes.Count; i++)
        {
            patrolPath[i] = patrolNodes[i].transform.position;
        }

        foreach (Waypoint waypoint in patrolNodes)
        {
            if (waypoint != null)
            {
                Destroy(waypoint.gameObject);
            }
        }
    }

    private void Update()
    {
        if (player != null)
        {
            bool playerVisible = CanSeePlayer();
            bool playerOnNavMesh = IsOnNavMesh(player.position);
            chasing = playerVisible && playerOnNavMesh;
        }

        if (chasing && player != null)
        {
            agent.SetDestination(player.position);
            return;
        }

        Patrol();
    }

    private void Patrol()
    {
        if (patrolPath.Length == 0) return;

        if (!agent.pathPending && agent.remainingDistance <= reachDistance)
        {
            patrolIndex = (patrolIndex + 1) % patrolPath.Length;
            agent.SetDestination(patrolPath[patrolIndex]);
        }
    }

    private bool CanSeePlayer()
    {
        Vector3 viewOrigin;

        if (eyePoint != null)
        {
            viewOrigin = eyePoint.position;
        }
        else
        {
            viewOrigin = transform.position;
        }

        Vector3 directionToPlayer = player.position - viewOrigin;
        float distanceToPlayer = directionToPlayer.magnitude;

        if (distanceToPlayer > viewDistance) return false;

        float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer.normalized);
        if (angleToPlayer > viewAngle * 0.5f) return false;

        if (Physics.Raycast(viewOrigin, directionToPlayer.normalized, distanceToPlayer, obstacleMask))
        {
            return false;
        }

        return true;
    }

    private bool IsOnNavMesh(Vector3 worldPosition)
    {
        return NavMesh.SamplePosition(worldPosition, out _, 1.0f, NavMesh.AllAreas);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Player")) return;

        Rigidbody playerRigidbody = collision.rigidbody;
        if (playerRigidbody == null) return;

        Vector3 knockbackDirection = (collision.transform.position - transform.position).normalized;
        playerRigidbody.AddForce(knockbackDirection * knockbackForce, ForceMode.Impulse);
    }
}
