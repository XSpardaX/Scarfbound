using UnityEngine;

public abstract class EnemyBase : MonoBehaviour
{
    // Default contact behaviour: deal 1 damage. Subclasses override for custom logic.
    public virtual void OnPlayerContact(Player player)
    {
        PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
        if (playerHealth == null || playerHealth.IsInvincible) return;

        playerHealth.TakeDamage(1);
    }

    // Fires while the player is overlapping a trigger collider on this enemy.
    // OnTriggerStay runs at FixedUpdate rate; iframes in PlayerHealth gate damage spam.
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
