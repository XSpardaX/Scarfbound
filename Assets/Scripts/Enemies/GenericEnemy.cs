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
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        BuildPatrolPath();

        if (patrolPath.Length > 0)
            agent.SetDestination(patrolPath[0]);
    }

    private void BuildPatrolPath()
    {
        var nodes = new List<Waypoint>();
        var visited = new HashSet<Waypoint>();

        // Walk the linked list, stopping if we hit null or revisit a node (handles cyclic chains).
        Waypoint cur = startNode;
        while (cur != null && visited.Add(cur))
        {
            nodes.Add(cur);
            cur = cur.nextNode;
        }

        patrolPath = new Vector3[nodes.Count];
        for (int i = 0; i < nodes.Count; i++)
            patrolPath[i] = nodes[i].transform.position;

        // Detach the waypoint GameObjects so they don't follow the enemy as it moves.
        foreach (var wp in nodes)
        {
            if (wp != null) Destroy(wp.gameObject);
        }
    }

    void Update()
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

    void Patrol()
    {
        if (patrolPath.Length == 0) return;

        if (!agent.pathPending && agent.remainingDistance <= reachDistance)
        {
            patrolIndex = (patrolIndex + 1) % patrolPath.Length;
            agent.SetDestination(patrolPath[patrolIndex]);
        }
    }

    bool CanSeePlayer()
    {
        Vector3 origin = eyePoint ? eyePoint.position : transform.position;
        Vector3 dirToPlayer = (player.position - origin);
        float distance = dirToPlayer.magnitude;

        if (distance > viewDistance)
            return false;

        float angle = Vector3.Angle(transform.forward, dirToPlayer.normalized);
        if (angle > viewAngle * 0.5f)
            return false;

        if (Physics.Raycast(origin, dirToPlayer.normalized, distance, obstacleMask))
            return false;

        return true;
    }

    bool IsOnNavMesh(Vector3 pos)
    {
        return NavMesh.SamplePosition(pos, out _, 1.0f, NavMesh.AllAreas);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Player")) return;

        Rigidbody rb = collision.rigidbody;
        if (rb == null) return;

        Vector3 knockDir = (collision.transform.position - transform.position).normalized;
        rb.AddForce(knockDir * knockbackForce, ForceMode.Impulse);
    }
}
