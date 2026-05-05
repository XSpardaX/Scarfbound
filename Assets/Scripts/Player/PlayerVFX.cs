using UnityEngine;

public class PlayerVFX : MonoBehaviour
{
    public GameObject runEffect;
    public Transform leftFootSpawnPoint;
    public Transform rightFootSpawnPoint;
    public float vfxLifetime = 0.3f;

    public void SpawnLeftFootEffect()
    {
        SpawnEffect(leftFootSpawnPoint);
    }

    public void SpawnRightFootEffect()
    {
        SpawnEffect(rightFootSpawnPoint);
    }

    private void SpawnEffect(Transform spawnPoint)
    {
        if (runEffect == null) return;

        Vector3 spawnPosition;
        if (spawnPoint != null)
        {
            spawnPosition = spawnPoint.position;
        }
        else
        {
            spawnPosition = transform.position;
        }

        GameObject dustInstance = Instantiate(runEffect, spawnPosition, Quaternion.identity);
        Destroy(dustInstance, vfxLifetime);
    }
}
