using System.Collections;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public Transform point1;       
    public Transform point2;       
    public float speed = 2f;
    public float waitTime = 2f;

    private Vector3 target;
    private bool isWaiting = false;

    void Start()
    {
        target = point1.position;
    }

    void Update()
    {
        if (isWaiting == false)
        {
            transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);

            if (Vector3.Distance(transform.position, target) < 0.01f)
            {
                StartCoroutine(WaitAndSwitchTarget());
            }
        }
    }

    IEnumerator WaitAndSwitchTarget()
    {
        isWaiting = true;
        yield return new WaitForSeconds(waitTime);

        target = (target == point1.position) ? point2.position : point1.position;
        isWaiting = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.transform.SetParent(transform);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.transform.SetParent(null);
        }
    }
}
