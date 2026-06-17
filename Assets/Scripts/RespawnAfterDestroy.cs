using System.Collections;
using UnityEngine;


public class RespawnAfterDestroy : MonoBehaviour
{
    public GameObject respawnPrefab;

    public float respawnDelay = 3f;

    public Transform respawnPoint;

    private Vector3 initialPosition;
    private Quaternion initialRotation;
    private bool appQuitting;

    private void Awake()
    {
        initialPosition = transform.position;
        initialRotation = transform.rotation;
    }

    private void OnApplicationQuit()
    {
        appQuitting = true;
    }

    private void OnDestroy()
    {
        if (appQuitting) return;
        if (!Application.isPlaying) return;
        if (respawnPrefab == null) return;

        RespawnCoroutineRunner.Run(RespawnRoutine());
    }

    private IEnumerator RespawnRoutine()
    {
        if (respawnDelay > 0f)
        {
            yield return new WaitForSeconds(respawnDelay);
        }

        Vector3 spawnPosition = respawnPoint != null ? respawnPoint.position : initialPosition;
        Quaternion spawnRotation = respawnPoint != null ? respawnPoint.rotation : initialRotation;

        Instantiate(respawnPrefab, spawnPosition, spawnRotation);
    }

    private sealed class RespawnCoroutineRunner : MonoBehaviour
    {
        private static RespawnCoroutineRunner instance;

        public static void Run(IEnumerator routine)
        {
            if (instance == null)
            {
                GameObject runnerObject = new GameObject("RespawnCoroutineRunner");
                DontDestroyOnLoad(runnerObject);
                instance = runnerObject.AddComponent<RespawnCoroutineRunner>();
            }

            instance.StartCoroutine(routine);
        }
    }
}
