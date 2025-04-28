using UnityEngine;

public class Moving : MonoBehaviour
{

    public Transform pointA;
    public Transform pointB;
    public float moveSpeed;

    private Vector3 nextposition;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        nextposition = pointB.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, nextposition, moveSpeed * Time.deltaTime);

        if(transform.position == nextposition)
        {
            nextposition = (nextposition == pointA.position) ? pointB.position : pointA.position;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.transform.parent = transform;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.transform.parent = null;
        }
    }
}
