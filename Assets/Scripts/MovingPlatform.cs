using UnityEngine;

[DefaultExecutionOrder(-100)]
public class MovingPlatform : MonoBehaviour
{
    public Transform[] points;
    public float speed = 2f;
    public float waitTimeAtEnds = 2f;

    private Vector3 velocity;
    private Vector3 movementDelta;

    private int currentIndex;
    private int direction = 1;

    private bool hasBeenActivated;
    private float waitTimer;
    private bool isWaiting;

    public Vector3 GetVelocity() => velocity;
    public Vector3 GetMovementDelta() => movementDelta;

    public void Activate()
    {
        if (hasBeenActivated) return;

        hasBeenActivated = true;
        isWaiting = false;
        waitTimer = 0f;
    }

    private void Start()
    {
        if (points == null || points.Length == 0) return;

        transform.position = points[0].position;
        currentIndex = 0;
    }

    private void FixedUpdate()
    {
        movementDelta = Vector3.zero;

        if (points == null || points.Length < 2) return;

        Vector3 previousPosition = transform.position;

        if (hasBeenActivated)
        {
            if (isWaiting)
            {
                waitTimer -= Time.fixedDeltaTime;
                if (waitTimer <= 0f)
                {
                    isWaiting = false;
                    direction *= -1;
                }
            }
            else
            {
                MoveAlongPath();
            }
        }

        movementDelta = transform.position - previousPosition;
        velocity = movementDelta / Time.fixedDeltaTime;
    }

    private void MoveAlongPath()
    {
        int nextIndex = currentIndex + direction;

        if (nextIndex < 0 || nextIndex >= points.Length)
        {
            BeginWaitAtEnd();
            return;
        }

        Vector3 targetPosition = points[nextIndex].position;
        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPosition,
            speed * Time.fixedDeltaTime
        );

        if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
        {
            currentIndex = nextIndex;

            if (currentIndex == 0 || currentIndex == points.Length - 1)
            {
                BeginWaitAtEnd();
            }
        }
    }

    private void BeginWaitAtEnd()
    {
        isWaiting = true;
        waitTimer = waitTimeAtEnds;
    }

    private void OnDrawGizmosSelected()
    {
        if (points == null || points.Length == 0) return;

        Gizmos.color = Color.yellow;
        for (int i = 0; i < points.Length; i++)
        {
            if (points[i] == null) continue;

            Gizmos.DrawWireSphere(points[i].position, 0.25f);

            if (i < points.Length - 1 && points[i + 1] != null)
            {
                Gizmos.DrawLine(points[i].position, points[i + 1].position);
            }
        }
    }
}
