using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : MonoBehaviour
{

    [Header("HorAbility Settings")]
    [SerializeField] private float shortHopForce = 6f; // Fixed low jump force
    [SerializeField] private float airMoveSpeed = 8f; // Faster horizontal control in air      //JACK DONT TOUCH THESE THEY ARE SO USELESS


    [Header("Movement")]
    [SerializeField] private float jumpForce = 8f; // Base force applied when jumping
    [SerializeField] private float moveSpeed = 5f; // Speed at which the character moves horizontally
    [SerializeField] private float maxJumpMultiplier = 3f; // Maximum multiplier for jump force
    [SerializeField] private float jumpChargeSpeed = 5f; // Speed at which the jump charge increases
    [SerializeField] private float jumpDecaySpeed = 8f; // Speed at which the jump multiplier decays after release
    [SerializeField] private float jumpReductionFactor = 5f; // Jump Decay For The Horizontal Ability

    [Header("State Settings")]
    [SerializeField] private bool isGrounded; // Whether the character is on the ground
    [SerializeField] private Rigidbody2D rb; // Reference to the Rigidbody2D component
    [SerializeField] private float jumpMult = 2f; // Current jump multiplier
    [SerializeField] private bool isPreppingJump; // Whether the character is charging a jump
    [SerializeField] private Vector2 jumpDirection = Vector2.up; // Direction of the jump, default is upwards
    [SerializeField] private bool isHorAbilityActive; // Whether the horizontal ability is active

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        HandleMovement();
        HandleJump();
        GravityAbility();
        HorAbility();
    }


    private void HandleMovement()
    {
        float moveInput = Input.GetAxis("Horizontal");

        // Adjustable jump direction (unchanged)
        if (moveInput < 0) jumpDirection = new Vector2(-0.25f, 1f).normalized;
        else if (moveInput > 0) jumpDirection = new Vector2(0.25f, 1f).normalized;

        // Movement logic
        if (isGrounded)
        {
            // Normal ground movement
            rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);
        }
        else if (isHorAbilityActive)
        {
            // Precise air control when ability is active
            rb.linearVelocity = new Vector2(moveInput * airMoveSpeed, rb.linearVelocity.y);
        }
    }

    private void HandleJump()
    {
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            if (isHorAbilityActive)
            {
                // Short hop with minimal vertical force
                rb.AddForce(Vector2.up * shortHopForce, ForceMode2D.Impulse);
                // Give immediate horizontal control
                float moveInput = Input.GetAxis("Horizontal");
                rb.linearVelocity = new Vector2(moveInput * airMoveSpeed, rb.linearVelocity.y);
            }
            else
            {
                // Normal charged jump
                isPreppingJump = true;
            }
        }

        // Normal charge (only when ability is OFF)
        if (!isHorAbilityActive)
        {
            if (Input.GetButton("Jump") && isPreppingJump) ChargeJump();
            else if (isPreppingJump) ExecuteJump();
            DecayJumpMultiplier();
        }
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
            float finalJumpForce = jumpForce;

            if (isHorAbilityActive)
            {
                finalJumpForce *= jumpReductionFactor;
            }

            finalJumpForce *= jumpMult;

            rb.AddForce(jumpDirection * finalJumpForce, ForceMode2D.Impulse);
            isPreppingJump = false;
            Debug.Log("Jump executed with force: " + (jumpDirection * finalJumpForce));
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

    // Abilities

    private void GravityAbility()
    {
        if(Input.GetKeyDown(KeyCode.Q)) // Press Q To Enable Gravity Ability
        {
            Physics2D.gravity = new Vector2(0, -5f);
        }
        else if (Input.GetKeyDown(KeyCode.E)) // Press E To Revert To Base Gravity
        {
            Physics2D.gravity = new Vector2(0, -9.8f);
        }
    }

    private void HorAbility()
    {
        if (Input.GetKeyDown(KeyCode.H)) 
        {
            isHorAbilityActive = true;
            Debug.Log("Horizontal Ability Activated");
        }
        else if (Input.GetKeyDown(KeyCode.J))   //PURELY KEYBINDS AND DEBUG DONT CHANGE
        {
            isHorAbilityActive = false;
            Debug.Log("Horizontal Ability Deactivated");
        } 
    }
}