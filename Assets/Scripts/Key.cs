using UnityEngine;

public class Key : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        Player collectingPlayer = other.GetComponent<Player>();
        if (collectingPlayer == null) return;

        collectingPlayer.hasKey = true;
        gameObject.SetActive(false);
    }
}
