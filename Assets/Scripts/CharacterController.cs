using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    // Serialized fields for adjustable parameters in the Unity Inspector
    [SerializeField] private float jumpForce = 8f; // Base force applied when jumping
    [SerializeField] private float moveSpeed = 5f; // Speed at which the character moves horizontally
    [SerializeField] private float maxJumpMultiplier = 3f; // Maximum multiplier for jump force
    [SerializeField] private float jumpChargeSpeed = 5f; // Speed at which the jump charge increases
    [SerializeField] private float jumpDecaySpeed = 8f; // Speed at which the jump multiplier decays after release

    // Private variables for internal state management
    private bool isGrounded; // Whether the character is on the ground
    private Rigidbody2D rb; // Reference to the Rigidbody2D component
    private float jumpMult = 2f; // Current jump multiplier
    private bool isPreppingJump; // Whether the character is charging a jump
    private Vector2 jumpDirection = Vector2.up; // Direction of the jump, default is upwards

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        HandleMovement();
        HandleJump();
    }

    private void HandleMovement()
    {
        if (!isGrounded || isPreppingJump)
            return;

        // Get horizontal input
        float moveInput = Input.GetAxis("Horizontal");

        // Adjust jump direction based on movement input
        if (moveInput < 0)
        {
            jumpDirection = new Vector2(-0.25f, 1f).normalized; // Jump slightly to the left
        }
        else if (moveInput > 0)
        {
            jumpDirection = new Vector2(0.25f, 1f).normalized; // Jump slightly to the right
        }
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);
    }

    private void HandleJump()
    {
        // Start charging the jump when the jump button is pressed and the character is grounded
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            isPreppingJump = true;
        }

        // Charge the jump while the jump button is held down
        if (Input.GetButton("Jump") && isPreppingJump)
        {
            ChargeJump();
        }
        // Execute the jump when the jump button is released
        else if (isPreppingJump)
        {
            ExecuteJump();
        }

        DecayJumpMultiplier();
    }

    private void ChargeJump()
    {
        // Increase the jump multiplier up to the maximum value
        if (jumpMult < maxJumpMultiplier)
        {
            jumpMult += jumpChargeSpeed * Time.deltaTime;
        }
    }

    private void ExecuteJump()
    {
        if (isGrounded)
        {
            rb.AddForce(jumpDirection * jumpForce * jumpMult, ForceMode2D.Impulse);
            isPreppingJump = false;
            Debug.Log("Jump executed with force: " + (jumpDirection * jumpForce * jumpMult));
        }
    }

    private void DecayJumpMultiplier()
    {
        // Gradually decrease the jump multiplier when not charging a jump
        if (!isPreppingJump && jumpMult > 1f)
        {
            jumpMult -= jumpDecaySpeed * Time.deltaTime;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Check all contact points 
        foreach (ContactPoint2D cp in collision.contacts)
        {
            if (cp.normal.y > 0.9f) // If the contact normal is mostly upwards, the character is grounded
            {
                isGrounded = true;
                Debug.Log("Grounded");
                break;
            }

            // Handle collision with walls
            if (collision.gameObject.CompareTag("Wall"))
            {
                Debug.Log("Wall Hit");
                Rigidbody rb = GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.linearVelocity = new Vector3(-rb.linearVelocity.x, rb.linearVelocity.y, -rb.linearVelocity.z);
                }
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        // Set the character as not grounded 
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
            Debug.Log("Not Grounded");
        }
    }
}