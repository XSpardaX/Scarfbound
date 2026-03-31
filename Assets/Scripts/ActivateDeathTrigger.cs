using UnityEngine;

public class ActivateDeathTrigger : MonoBehaviour
{
    public GameObject trigger;
    public Player player;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            trigger.SetActive(true);
        }
    }
}
