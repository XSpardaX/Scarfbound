using UnityEngine;

public class FlyingEnemy : EnemyBase
{
    public enum PatrolAxis { X, Z, Y }

    public PatrolAxis patrolAxis = PatrolAxis.Y;
    public float patrolDistance = 3f;
    public float moveSpeed = 2f;

    public float stompBounceForce = 10f;
    public float stompHeightOffset = 0.3f;

    private Vector3 startPosition;
    private float patrolTimer;

    private void Awake()
    {
        startPosition = transform.position;
        patrolTimer = 0f;
    }

    private void Update()
    {
        patrolTimer += Time.deltaTime * moveSpeed / patrolDistance;

        float lerpAmount = (Mathf.Sin(patrolTimer) + 1f) * 0.5f;

        Vector3 patrolOffset = Vector3.zero;

        switch (patrolAxis)
        {
            case PatrolAxis.X:
                patrolOffset.x = Mathf.Lerp(-patrolDistance, patrolDistance, lerpAmount);
                break;
            case PatrolAxis.Z:
                patrolOffset.z = Mathf.Lerp(-patrolDistance, patrolDistance, lerpAmount);
                break;
            case PatrolAxis.Y:
                patrolOffset.y = Mathf.Lerp(-patrolDistance, patrolDistance, lerpAmount);
                break;
        }

        transform.position = startPosition + patrolOffset;
    }

    public override void OnPlayerContact(Player player)
    {
        bool playerMovingDown = player.VerticalVelocity < 0f;
        float playerTop = player.transform.position.y;
        float enemyTop = transform.position.y + stompHeightOffset;

        if (playerTop > enemyTop && playerMovingDown)
        {
            player.ApplyBounce(stompBounceForce);
            return;
        }

        PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
        if (playerHealth == null || playerHealth.IsInvincible) return;

        playerHealth.TakeDamage(1);
    }
}
