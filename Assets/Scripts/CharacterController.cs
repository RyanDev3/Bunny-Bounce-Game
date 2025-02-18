using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    [SerializeField]
    [Range(1, 5000)] float jump = 1;

    public float moveSpeed = 5f;
    public float jumpForce = 10f;
    private bool isGrounded;
    private Rigidbody2D rb;
    private BoxCollider2D box2d;
    private float jumpMult = 1f;
    private float jumpMultMax = 3f;
    private bool isPreppingJump;
    private Vector2 mouseDirection;

    void Start()
    {
        Application.targetFrameRate = 60;

        rb = GetComponent<Rigidbody2D>();
        box2d = GetComponent<BoxCollider2D>();
    }

    void Update()
    {
        GetMousePositionWorldSpace();

        // Handle horizontal movement
        float moveInput = Input.GetAxis("Horizontal");
        
        if(moveInput != 0)
            rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);

        // Handle jumping
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            
        }
        if (Input.GetButton("Jump"))
        {
            isPreppingJump = true;

            if(jumpMult < jumpMultMax)
            {
                jumpMult += 4 * Time.deltaTime;
            }
        }
        else
        {           
            if (isGrounded && isPreppingJump)
            {
                
                isPreppingJump = false;
                AddForceJump(jumpMult);
                //rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce * jumpMult);
            }

            if (jumpMult > 1)
            {
                jumpMult -= 8 * Time.deltaTime;
            }
        }

        Debug.DrawLine(transform.position, Camera.main.ScreenToWorldPoint(Input.mousePosition), Color.red);
    }

    void AddForceJump(float multiplier)
    {
        print("Force: " + jump *  multiplier);

        rb.AddForce(mouseDirection.normalized * jump * multiplier);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        foreach (ContactPoint2D cp in collision.contacts)
        {
            if(cp.normal == new Vector2(0,1))
            {
                isGrounded = true;
            }
        }

        // Check if the player is on the ground
        //if (collision.gameObject.CompareTag("Ground"))
        //{
            
        //}
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        // Check if the player is no longer on the ground
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }

    void GetMousePositionWorldSpace()
    {
        mouseDirection = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position).normalized;
    }

    private void OnDrawGizmos()
    {
        
    }
}