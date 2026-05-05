using UnityEngine;

[RequireComponent(typeof(Collider))]
public class EnemySpawnTrigger : MonoBehaviour
{
    public EnemyTypes type;
    public Transform spawnPoint;
    public bool oneShot = true;
    public EnemyFactory factory;

    private bool spawned;

    private void Awake()
    {
        if (factory == null)
            factory = FindAnyObjectByType<EnemyFactory>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (oneShot && spawned) return;

        if (factory == null)
        {
            Debug.LogError($"[{name}] No EnemyFactory found in scene.");
            return;
        }

        Transform sp = spawnPoint != null ? spawnPoint : transform;
        factory.SpawnEnemy(type, sp.position, sp.rotation);
        spawned = true;
    }

    private void OnDrawGizmosSelected()
    {
        if (spawnPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(spawnPoint.position, 0.4f);
        Gizmos.DrawLine(transform.position, spawnPoint.position);
    }
}
