using Unity.VisualScripting;
using UnityEngine;

public class Key : MonoBehaviour
{
    public Player player;
    private void Start()
    {
        player = GetComponent<Player>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Player player = other.GetComponent<Player>();
            if (player != null)
            {
                gameObject.SetActive(false);
                player.hasKey = true;
            }
        }
    }
}
