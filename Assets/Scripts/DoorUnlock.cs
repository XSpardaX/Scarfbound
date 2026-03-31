using UnityEngine;

public class DoorUnlock : MonoBehaviour
{
    public GameObject door;   
    public Player player;    

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (player.hasKey == true)
            {
                door.SetActive(false);
            }
        }
    }
}