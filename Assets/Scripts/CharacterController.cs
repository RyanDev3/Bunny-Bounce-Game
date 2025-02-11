using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 10f;
    private bool isGrounded;
    private Rigidbody2D rb;
    private BoxCollider2D box2d;
    private float jumpMult = 1f;
    private float jumpMultMax = 3f;
    private bool isPreppingJump;


    void Start()
    {
        Application.targetFrameRate = 60;

        rb = GetComponent<Rigidbody2D>();
        box2d = GetComponent<BoxCollider2D>();
    }

    void Update()
    {
        // Handle horizontal movement
        float moveInput = Input.GetAxis("Horizontal");
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
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce * jumpMult);
            }

            if (jumpMult > 0)
            {
                jumpMult -= 8 * Time.deltaTime;
            }
        }

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
}