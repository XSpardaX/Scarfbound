using UnityEngine;

public class Key : MonoBehaviour
{
    public float rotationSpeed = 90f;

    private void Update()
    {
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.World);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        Player collectingPlayer = other.GetComponent<Player>();
        if (collectingPlayer == null) return;

        collectingPlayer.hasKey = true;
        if (SfxManager.Instance != null) SfxManager.Instance.Play(SfxIds.KeyCollect);
        gameObject.SetActive(false);
    }
}
