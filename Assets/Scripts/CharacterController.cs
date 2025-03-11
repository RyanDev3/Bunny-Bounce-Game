using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    [SerializeField] private float jumpForce = 8f; 
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float maxJumpMultiplier = 3f;
    [SerializeField] private float jumpChargeSpeed = 5f;
    [SerializeField] private float jumpDecaySpeed = 8f;

    private bool isGrounded;
    private Rigidbody2D rb;
    private float jumpMult = 2f;
    private bool isPreppingJump;
    private Vector2 jumpDirection = Vector2.up; 

    void Start()
    {
        Application.targetFrameRate = 160;
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        HandleMovement();
        HandleJump();
    }

    private void HandleMovement()
    {
        if (!isGrounded)
            return;

        if (isGrounded)
            return;

        if (isPreppingJump)
            return;

        float moveInput = Input.GetAxis("Horizontal");

        if (moveInput < 0)
        {
            jumpDirection = new Vector2(-0.25f, 1f).normalized;
        }
        else if (moveInput > 0)
        {
            jumpDirection = new Vector2(0.25f, 1f).normalized;
        }

        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);
    }

    private void HandleJump()
    {
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            isPreppingJump = true;
        }

        if (Input.GetButton("Jump") && isPreppingJump)
        {
            ChargeJump();
        }
        else if (isPreppingJump)
        {
            ExecuteJump();
        }

        DecayJumpMultiplier();
    }

    private void ChargeJump()
    {
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
        if (!isPreppingJump && jumpMult > 1f)
        {
            jumpMult -= jumpDecaySpeed * Time.deltaTime;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        foreach (ContactPoint2D cp in collision.contacts)
        {
            if (cp.normal.y > 0.9f) 
            {
                isGrounded = true;
                Debug.Log("Grounded");
                break;
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
            Debug.Log("Not Grounded");
        }
    }
}