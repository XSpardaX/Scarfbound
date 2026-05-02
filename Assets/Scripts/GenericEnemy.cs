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

    private Waypoint currentNode;
    private NavMeshAgent agent;
    private Transform player;
    private bool chasing;

    public override void Initialize()
    {
        agent = GetComponent<NavMeshAgent>();
        currentNode = startNode;

        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (currentNode != null)
            agent.SetDestination(currentNode.transform.position);
    }

    void Update()
    {
        if (player != null)
        {
            bool playerVisible = CanSeePlayer();
            bool playerOnNavMesh = IsOnNavMesh(player.position);

            if (playerVisible && playerOnNavMesh)
            {
                chasing = true;
            }
            else if (!playerVisible || !playerOnNavMesh)
            {
                chasing = false;
            }
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
        if (currentNode == null) return;

        if (!agent.pathPending && agent.remainingDistance <= reachDistance)
        {
            if (currentNode.nextNode != null)
            {
                currentNode = currentNode.nextNode;
                agent.SetDestination(currentNode.transform.position);
            }
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
        NavMeshHit hit;
        return NavMesh.SamplePosition(pos, out hit, 1.0f, NavMesh.AllAreas);
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