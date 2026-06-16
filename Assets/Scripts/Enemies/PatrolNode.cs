using System.Collections.Generic;
using UnityEngine;

public class PatrolNode : MonoBehaviour
{
    public List<PatrolNode> neighbors = new List<PatrolNode>();
    public float waitTime;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, 0.3f);

        if (neighbors == null) return;

        Gizmos.color = Color.green;
        foreach (PatrolNode neighbor in neighbors)
        {
            if (neighbor == null) continue;
            Gizmos.DrawLine(transform.position, neighbor.transform.position);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.position, 0.35f);
    }
}
