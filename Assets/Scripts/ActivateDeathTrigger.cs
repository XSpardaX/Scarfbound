using UnityEngine;

public class ActivateDeathTrigger : MonoBehaviour
{
    public GameObject trigger;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (trigger != null)
        {
            trigger.SetActive(true);
        }
    }
}
