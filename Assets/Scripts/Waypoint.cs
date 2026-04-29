using UnityEngine;

public class Waypoint : MonoBehaviour
{
    public Waypoint nextNode;

    private void OnDrawGizmos()
    {
        if (nextNode != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, nextNode.transform.position);
        }
    }
}