using UnityEngine;

public class PlatformGroupToggle : MonoBehaviour
{
    [Header("Timing")]
    public float activeTime = 2f;
    public float inactiveTime = 2f;

    [Header("Player Tag")]
    public string playerTag = "Player";

    private bool isRunning = false;
    private bool isActive = true;
    private float timer;

    void Start()
    {
        SetChildrenState(true);
    }

    void Update()
    {
        if (!isRunning) return;

        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            isActive = !isActive;
            timer = isActive ? activeTime : inactiveTime;

            SetChildrenState(isActive);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(playerTag)) return;

        if (!isRunning)
        {
            isRunning = true;
            isActive = true;
            timer = activeTime;

            SetChildrenState(true);
        }
    }

    void SetChildrenState(bool state)
    {
        foreach (Renderer r in GetComponentsInChildren<Renderer>())
        {
            r.enabled = state;
        }

        foreach (Collider c in GetComponentsInChildren<Collider>())
        {
            if (!c.isTrigger)
                c.enabled = state;
        }
    }
}