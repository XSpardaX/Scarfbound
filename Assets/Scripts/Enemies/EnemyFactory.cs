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

    private Dictionary<EnemyTypes, GameObject> map;

    private void Awake()
    {
        map = new Dictionary<EnemyTypes, GameObject>();
        foreach (var e in entries)
        {
            if (e.prefab == null)
            {
                Debug.LogWarning($"[EnemyFactory] No prefab assigned for {e.type}");
                continue;
            }
            map[e.type] = e.prefab;
        }
    }

    public EnemyBase SpawnEnemy(EnemyTypes type, Vector3 position)
    {
        return SpawnEnemy(type, position, Quaternion.identity);
    }

    public EnemyBase SpawnEnemy(EnemyTypes type, Vector3 position, Quaternion rotation)
    {
        if (!map.TryGetValue(type, out var prefab))
        {
            Debug.LogError($"[EnemyFactory] Missing prefab for {type}");
            return null;
        }

        GameObject obj = Instantiate(prefab, position, rotation);
        EnemyBase enemy = obj.GetComponent<EnemyBase>();

        if (enemy == null)
        {
            Debug.LogError($"[EnemyFactory] Spawned prefab {prefab.name} has no EnemyBase component.");
        }

        return enemy;
    }
}
