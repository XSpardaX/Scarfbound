using UnityEngine;

public class GenericEnemy : MonoBehaviour
{
    public enum PatrolAxis { X, Z, Y }

    public PatrolAxis patrolAxis = PatrolAxis.X;
    public float patrolDistance = 3f;
    public float moveSpeed = 2f;

    public float stompBounceForce = 10f;
    public float stompHeightOffset = 0.3f;

    private Vector3 startPosition;
    private float patrolTimer;

    private void Start()
    {
        startPosition = transform.position;
    }

    private void Update()
    {
        patrolTimer += Time.deltaTime * moveSpeed / patrolDistance;
        float t = (Mathf.Sin(patrolTimer) + 1f) * 0.5f; // 0 to 1
        Vector3 offset = Vector3.zero;
        switch (patrolAxis)
        {
            case PatrolAxis.X: offset.x = Mathf.Lerp(-patrolDistance, patrolDistance, t); break;
            case PatrolAxis.Z: offset.z = Mathf.Lerp(-patrolDistance, patrolDistance, t); break;
            case PatrolAxis.Y: offset.y = Mathf.Lerp(-patrolDistance, patrolDistance, t); break;
        }
        transform.position = startPosition + offset;
    }

    public void OnPlayerHit(Transform player, Vector3 contactPoint, bool playerMovingDown)
    {
        float playerTop = player.position.y;
        float enemyTop = transform.position.y + stompHeightOffset;

        if (playerTop > enemyTop && playerMovingDown)
        {
            OnStomped(player);
        }
        else
        {
            OnSideHit(player);
        }
    }

    private void OnStomped(Transform player)
    {
        Player pc = player.GetComponent<Player>();
        if (pc != null)
            pc.ApplyBounce(stompBounceForce);
    }

    private void OnSideHit(Transform player)
    {
        if (!player.CompareTag("Player"))
            return;

        PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
        if (playerHealth != null && !playerHealth.IsInvincible)
            playerHealth.TakeDamage(1);
    }

}
