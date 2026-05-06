using UnityEngine;

public abstract class EnemyBase : MonoBehaviour
{
    public virtual void OnPlayerContact(Player player)
    {
        PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
        if (playerHealth == null || playerHealth.IsInvincible) return;

        playerHealth.TakeDamage(1);
    }

    private void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        Player touchingPlayer = other.GetComponent<Player>();
        if (touchingPlayer != null)
        {
            OnPlayerContact(touchingPlayer);
        }
    }
}
