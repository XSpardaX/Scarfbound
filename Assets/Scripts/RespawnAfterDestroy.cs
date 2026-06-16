using System.Collections;
using UnityEngine;

/// <summary>
/// Attach to an object that may be destroyed at runtime.
/// When this object is destroyed, a prefab copy is spawned after a delay.
/// </summary>
public class RespawnAfterDestroy : MonoBehaviour
{
    [Tooltip("Prefab to spawn after this object is destroyed.")]
    public GameObject respawnPrefab;

    [Tooltip("Seconds to wait before spawning the replacement.")]
    public float respawnDelay = 3f;

    [Tooltip("Optional explicit spawn point. If empty, original position is used.")]
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
