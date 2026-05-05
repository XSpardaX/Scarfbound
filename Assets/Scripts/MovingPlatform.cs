using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public Transform[] points;
    public float speed = 2f;

    public Vector3 detectionBoxSize = new Vector3(2f, 1f, 2f);
    public Vector3 detectionOffset = new Vector3(0f, 1f, 0f);

    private Vector3 velocity;

    private int currentIndex = 0;
    private int direction = 1;

    private bool playerOnPlatform = false;
    private bool isMoving = false;

    public Vector3 GetVelocity()
    {
        return velocity;
    }

    private void Start()
    {
        if (points.Length == 0) return;

        transform.position = points[0].position;
        currentIndex = 0;
    }

    private void Update()
    {
        DetectPlayer();
    }

    private void FixedUpdate()
    {
        if (!isMoving || points.Length == 0) return;

        int nextIndex = currentIndex + direction;
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

            if (currentIndex == 0 || currentIndex == points.Length - 1)
            {
                isMoving = false;
                direction *= -1;
            }
        }
    }

    private void DetectPlayer()
    {
        Vector3 boxCenter = transform.position + detectionOffset;
        Collider[] nearbyColliders = Physics.OverlapBox(boxCenter, detectionBoxSize * 0.5f);

        bool foundPlayer = false;

        foreach (Collider nearby in nearbyColliders)
        {
            if (nearby.CompareTag("Player"))
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

            // If the player leaves before the platform reaches its destination,
            // reverse direction so it heads back to where it started.
            if (isMoving)
            {
                direction *= -1;
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Vector3 boxCenter = transform.position + detectionOffset;
        Gizmos.DrawWireCube(boxCenter, detectionBoxSize);
    }
}
