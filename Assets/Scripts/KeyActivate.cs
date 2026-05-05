using UnityEngine;

public class KeyActivate : MonoBehaviour
{
    public GameObject obj;   
    public Player player;    

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (player.hasKey == true)
            {
                obj.SetActive(true);
            }
        }
    }
}