using UnityEngine;

public class Wisps : MonoBehaviour
{
    public int wispValue = 1;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
        if (playerHealth == null) return;

        playerHealth.AddWisps(wispValue);
        if (SfxManager.Instance != null) SfxManager.Instance.Play(SfxIds.Collect);
        gameObject.SetActive(false);
    }
}
