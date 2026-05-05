using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public Transform checkpointLocation;

    private bool isActivated;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (isActivated) return;
        if (CheckpointManager.Instance == null) return;

        CheckpointManager.Instance.SetCheckpoint(checkpointLocation.position);
        isActivated = true;

        if (CheckpointIndicator.Instance != null)
        {
            CheckpointIndicator.Instance.ShowIndicator();
        }
    }
}
