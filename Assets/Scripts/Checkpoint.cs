using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private bool isActivated;
    public Transform checkpointLocation;

    private void OnTriggerEnter(Collider player)
    {
        if (!player.CompareTag("Player")) 
            return;

        if (isActivated ==  true) 
            return;

        if (CheckpointManager.Instance != null)
        {
            CheckpointManager.Instance.SetCheckpoint(checkpointLocation.position);
            isActivated = true;

            CheckpointIndicator.Instance.ShowIndicator();

            Debug.Log("checkpoint");
        }
    }
}

