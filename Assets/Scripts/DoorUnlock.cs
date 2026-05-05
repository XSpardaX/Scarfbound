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

        door.SetActive(false);
    }
}
