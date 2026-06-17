using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 10f;
    public float lifetime = 5f;
    public int damage = 1;

    private Vector3 convergeDirection;
    private float convergeStrength;
    private float convergeTurnRate;
    private bool hasImpacted;

    private void Awake()
    {
        var rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.isKinematic = true;

        GetComponent<Collider>().isTrigger = true;
    }

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }

    public void SetConvergence(Vector3 direction, float strength, float turnRate)
    {
        convergeDirection = direction;
        convergeDirection.y = 0f;
        if (convergeDirection.sqrMagnitude > 0.0001f)
        {
            convergeDirection.Normalize();
        }

        convergeStrength = Mathf.Clamp01(strength);
        convergeTurnRate = turnRate;
    }

    private void Update()
    {
        if (convergeStrength > 0f && convergeDirection.sqrMagnitude > 0.0001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(convergeDirection, Vector3.up);
            float turnDegrees = convergeTurnRate * convergeStrength * Time.deltaTime;
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, turnDegrees);
        }

        transform.position += transform.forward * speed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hasImpacted) return;

        PlayerHealth health = null;
        if (other.CompareTag("Player"))
        {
            health = other.GetComponent<PlayerHealth>();
            if (health == null)
            {
                health = other.GetComponentInParent<PlayerHealth>();
            }
        }
        else
        {
            health = other.GetComponentInParent<PlayerHealth>();
            if (health == null || !other.transform.root.CompareTag("Player"))
            {
                health = null;
            }
        }

        if (health != null)
        {
            if (!health.IsInvincible && !health.IsDead)
            {
                health.TakeDamage(damage);
            }

            hasImpacted = true;
            Destroy(gameObject);
            return;
        }

        if (other.CompareTag("Wisp"))
        {
            CorruptibleWisp corruptibleWisp = other.GetComponent<CorruptibleWisp>();
            if (corruptibleWisp == null)
            {
                corruptibleWisp = other.GetComponentInParent<CorruptibleWisp>();
            }

            if (corruptibleWisp != null && corruptibleWisp.TryCorrupt(this))
            {
                hasImpacted = true;
                Destroy(gameObject);
                return;
            }
        }

        if (other.isTrigger)
        {
            return;
        }

        hasImpacted = true;
        Destroy(gameObject);
    }
}
