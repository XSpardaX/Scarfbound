using UnityEngine;

public class KeyActivate : MonoBehaviour
{
    public GameObject objectToActivate;
    public Player player;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (player == null) return;
        if (!player.hasKey) return;

        if (objectToActivate != null)
        {
            objectToActivate.SetActive(true);
        }
    }
}
