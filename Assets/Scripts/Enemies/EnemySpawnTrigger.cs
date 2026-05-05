using UnityEngine;

[RequireComponent(typeof(Collider))]
public class EnemySpawnTrigger : MonoBehaviour
{
    public EnemyTypes type;
    public Transform spawnPoint;
    public bool oneShot = true;
    public EnemyFactory factory;

    private bool hasSpawned;

    private void Awake()
    {
        if (factory == null)
        {
            factory = FindAnyObjectByType<EnemyFactory>();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (oneShot && hasSpawned) return;

        if (factory == null)
        {
            Debug.LogError($"[{name}] No EnemyFactory found in scene.");
            return;
        }

        Transform spawnTransform;

        if (spawnPoint != null)
        {
            spawnTransform = spawnPoint;
        }
        else
        {
            spawnTransform = transform;
        }

        factory.SpawnEnemy(type, spawnTransform.position, spawnTransform.rotation);
        hasSpawned = true;
    }

    private void OnDrawGizmosSelected()
    {
        if (spawnPoint == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(spawnPoint.position, 0.4f);
        Gizmos.DrawLine(transform.position, spawnPoint.position);
    }
}
