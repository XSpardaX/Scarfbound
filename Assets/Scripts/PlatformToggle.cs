using UnityEngine;

public class PlatformGroupToggle : MonoBehaviour
{
    [Header("Timing")]
    public float activeTime = 2f;
    public float inactiveTime = 2f;

    [Header("Player Tag")]
    public string playerTag = "Player";

    private enum Phase { Idle, CountingDown, Inactive }
    private Phase phase = Phase.Idle;
    private float timer;

    void Start()
    {
        SetChildrenState(true);
    }

    void Update()
    {
        if (phase == Phase.Idle) return;

        timer -= Time.deltaTime;
        if (timer > 0f) return;

        if (phase == Phase.CountingDown)
        {
            phase = Phase.Inactive;
            timer = inactiveTime;
            SetChildrenState(false);
        }
        else // Phase.Inactive
        {
            phase = Phase.Idle;
            SetChildrenState(true);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(playerTag)) return;
        if (phase != Phase.Idle) return;

        phase = Phase.CountingDown;
        timer = activeTime;
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
