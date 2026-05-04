using UnityEngine;

public class Wisps : MonoBehaviour 
{
    public int wispValue = 1;

    private bool collected = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.AddWisps(wispValue);
            collected = true;

           if (collected == true)
            {
                gameObject.SetActive(false);
            }
        }
    }
}
