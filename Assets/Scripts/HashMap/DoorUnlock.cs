using UnityEngine;

public class DoorUnlock : MonoBehaviour
{
    public GameObject door;
    public Player player;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (player == null) return;
        if (!player.hasKey) return;


        //Hash map
        if (SfxManager.Instance != null) 
            SfxManager.Instance.Play(SfxIds.DoorUnlock);

        door.SetActive(false);
    }
}
