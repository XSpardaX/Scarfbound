using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [Header("Waypoints")]
    public Transform[] points;

    [Header("Movement")]
    public float speed = 2f;

    [Header("Player Detection")]
    public Vector3 detectionBoxSize = new Vector3(2f, 1f, 2f);
    public Vector3 detectionOffset = new Vector3(0f, 1f, 0f);

    private int currentIndex = 0;
    private int direction = 1; // 1 = forward, -1 = backward

    private bool playerOnPlatform = false;
    private bool isMoving = false;

    private Vector3 velocity;

    public Vector3 GetVelocity()
    {
        return velocity;
    }

    void Start()
    {
        if (points.Length == 0) return;

        transform.position = points[0].position;
        currentIndex = 0;
    }

    void Update()
    {
        DetectPlayer();
    }

    void FixedUpdate()
    {
        if (!isMoving || points.Length == 0) return;

        int nextIndex = currentIndex + direction;

        // Safety clamp
        nextIndex = Mathf.Clamp(nextIndex, 0, points.Length - 1);

        Vector3 targetPosition = points[nextIndex].position;

        Vector3 newPosition = Vector3.MoveTowards(
            transform.position,
            targetPosition,
            speed * Time.fixedDeltaTime
        );

        velocity = (newPosition - transform.position) / Time.fixedDeltaTime;
        transform.position = newPosition;

        if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
        {
            currentIndex = nextIndex;

            // ?? Reached an end ? stop and wait for player to step off/on
            if (currentIndex == 0 || currentIndex == points.Length - 1)
            {
                isMoving = false;

                // Flip direction ONLY at ends
                direction *= -1;
            }
        }
    }

    void DetectPlayer()
    {
        Vector3 boxCenter = transform.position + detectionOffset;

        Collider[] hits = Physics.OverlapBox(boxCenter, detectionBoxSize * 0.5f);

        bool foundPlayer = false;

        foreach (var col in hits)
        {
            if (col.CompareTag("Player"))
            {
                foundPlayer = true;
                break;
            }
        }

        if (foundPlayer && !playerOnPlatform)
        {
            playerOnPlatform = true;

            if (!isMoving)
            {
                isMoving = true;
            }
        }

        else if (!foundPlayer && playerOnPlatform)
        {
            playerOnPlatform = false;

            if (currentIndex != 0 && currentIndex != points.Length - 1)
            {
                direction *= -1;
                isMoving = true;
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Vector3 boxCenter = transform.position + detectionOffset;
        Gizmos.DrawWireCube(boxCenter, detectionBoxSize);
    }
}