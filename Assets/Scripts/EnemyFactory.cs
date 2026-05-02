using UnityEngine;

public class EnemyFactory : MonoBehaviour
{
    public GameObject generic;
    public GameObject ranged;
    public GameObject flyer;

    public EnemyBase SpawnEnemy(EnemyTypes type, Vector3 position)
    {
        GameObject prefab = null;

        switch (type)
        {
            case EnemyTypes.Generic:
                prefab = generic;
                break;

            case EnemyTypes.Ranged:
                prefab = ranged;
                break;

            case EnemyTypes.Flying:
                prefab = flyer;
                break;
        }

        if (prefab == null)
        {
            Debug.LogError("Missing prefab for " + type);
            return null;
        }

        GameObject obj = Instantiate(prefab, position, Quaternion.identity);
        EnemyBase enemy = obj.GetComponent<EnemyBase>();

        if (enemy != null)
            enemy.Initialize();

        return enemy;
    }
}