using System.Collections.Generic;
using UnityEngine;

public class EnemyFactory : MonoBehaviour
{
    [System.Serializable]
    public struct EnemyEntry
    {
        public EnemyTypes type;
        public GameObject prefab;
    }

    public List<EnemyEntry> entries = new List<EnemyEntry>();

    private Dictionary<EnemyTypes, GameObject> prefabsByType;

    private void Awake()
    {
        prefabsByType = new Dictionary<EnemyTypes, GameObject>();

        foreach (EnemyEntry entry in entries)
        {
            if (entry.prefab == null)
            {
                Debug.LogWarning($"[EnemyFactory] No prefab assigned for {entry.type}");
                continue;
            }

            prefabsByType[entry.type] = entry.prefab;
        }
    }

    public EnemyBase SpawnEnemy(EnemyTypes type, Vector3 position)
    {
        return SpawnEnemy(type, position, Quaternion.identity);
    }

    public EnemyBase SpawnEnemy(EnemyTypes type, Vector3 position, Quaternion rotation)
    {
        if (!prefabsByType.TryGetValue(type, out GameObject prefab))
        {
            Debug.LogError($"[EnemyFactory] Missing prefab for {type}");
            return null;
        }

        GameObject spawnedEnemy = Instantiate(prefab, position, rotation);
        EnemyBase enemyComponent = spawnedEnemy.GetComponent<EnemyBase>();

        if (enemyComponent == null)
        {
            Debug.LogError($"[EnemyFactory] Spawned prefab {prefab.name} has no EnemyBase component.");
        }

        return enemyComponent;
    }
}
