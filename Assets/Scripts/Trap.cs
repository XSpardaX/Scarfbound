using UnityEngine;

public class Trap : MonoBehaviour
{
    public GameObject trap;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (trap != null)
        {
            trap.SetActive(true);
        }
    }
}
