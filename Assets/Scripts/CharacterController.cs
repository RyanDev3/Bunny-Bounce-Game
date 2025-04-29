using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharacterController : MonoBehaviour
{

    //private enum AbilityState { Normal, HorizontalAirControl, LowGravity }
    //private AbilityState currentAbility = AbilityState.Normal;


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
    [SerializeField] private float jumpAngleX = 0.25f;
    [SerializeField] private float jumpAngleY = 1f;
    [SerializeField] private float colourMult = 1f;
    [SerializeField] private Text abilityText;

    [SerializeField] private float bounceSpeed = 100;

    [SerializeField] private bool Test = true;


    Animator animator; 
    Dictionary<int, bool> abilities = new Dictionary<int, bool>()
    {
        {0, true }, // Normal
        {1, false }, // HorizontalAirControl
        {2, false } // LowGravity
    };
    int currentAbility = 0;

    private void OnEnable()
    {
        AbilityPickupManager.OnAbilityLevelIncrease += UnlockAbility;
    }

    private void OnDisable()
    {
        AbilityPickupManager.OnAbilityLevelIncrease -= UnlockAbility;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        HandleMovement();
        HandleJump();
        //HandleAbilities();
        SwitchAbility();
        ApplyCurrentAbility();
        animator.SetBool("isJumping", !isGrounded);

        animator.SetFloat("Xvelo", Mathf.Abs(rb.linearVelocity.x));
        animator.SetFloat("Yvelo", rb.linearVelocity.y);
    }


    private void HandleMovement()
    {

        float moveInput = Input.GetAxis("Horizontal");

        // Adjust jump direction (only affects next jump)
        if (moveInput < 0) jumpDirection = new Vector2(-jumpAngleX, jumpAngleY).normalized;
        else if (moveInput > 0) jumpDirection = new Vector2(jumpAngleX, jumpAngleY).normalized;

        // Only allow movement when grounded or when ability is active
        if (isGrounded)
        {

            //rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);
        }
        else if (isHorAbilityActive)
        {
            //rb.linearVelocity = new Vector2(moveInput * airMoveSpeed, rb.linearVelocity.y);
        }
    }

    private void HandleJump()
    {
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            if (isHorAbilityActive)
            {
                // Fixed-height jump with horizontal influence
                rb.linearVelocity = new Vector2(
                    Input.GetAxis("Horizontal") * airMoveSpeed * 0.5f,
                    shortHopForce
                );
            }
            else
            {
                // Normal charged jump
                isPreppingJump = true;
                rb.linearVelocity = Vector2.zero;
            }
        }

        if (!isHorAbilityActive)
        {
            if (Input.GetButton("Jump") && isPreppingJump)
            {
                ChargeJump();
                rb.linearVelocity = Vector2.zero; // Freeze during charge
            }
            else if (isPreppingJump)
            {
                ExecuteJump();
            }
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

            rb.AddForce(jumpDirection * (colourMult * finalJumpForce), ForceMode2D.Impulse);
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
                animator.SetBool("isJumping", !isGrounded);
                Debug.Log("Grounded");
                break;
            }
            else
            {

                Vector2 bounceDirection = cp.normal;

                rb.AddForce(bounceDirection * bounceSpeed);
                
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
    void SwitchAbility()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            IncrementAbility();
        }
    }

    void IncrementAbility()
    {
        if (currentAbility + 1 == abilities.Count)
        {
            currentAbility = 0;
        }
        else
        {
            currentAbility++;
        }

        if (!CheckAbility())
        {
            //print("Ability at " + currentAbility + " is locked. Moving to next ability");
            IncrementAbility();
        }
        else
        {
            //print("Current ability: " + currentAbility);
        }
    }

    bool CheckAbility()
    {
        return abilities[currentAbility];
    }

    void UnlockAbility(int index)
    {
        abilities[index] = true;
    }

    public void ApplyCurrentAbility()
    {
        string abilityName = "";

        switch (currentAbility)
        {
            case 0:
                jumpAngleX = 0.25f;
                jumpAngleY = 1f;
                colourMult = 1f;
                Physics2D.gravity = new Vector2(0, -9.8f);
                abilityName = "Vertical Jump";
                break;

            case 1:
                jumpAngleX = 0.75f;
                jumpAngleY = 0.5f;
                colourMult = 1f;
                Physics2D.gravity = new Vector2(0, -9.8f);
                abilityName = "Horizontal Jump";
                break;

            case 2:
                jumpAngleX = 0.35f;
                jumpAngleY = 0.75f;
                colourMult = 0.7f;
                Physics2D.gravity = new Vector2(0, -5f);
                abilityName = "Low Gravity Jump";
                break;
        }

        // Update the text if the reference exists
        if (abilityText != null)
        {
            abilityText.text = abilityName;
        }
        else
        {
            // Try to find the text component if not assigned
            abilityText = GetComponentInChildren<Text>();
            if (abilityText != null)
            {
                abilityText.text = abilityName;
            }
        }

        Debug.Log(abilityName);
    }

    //Old ability code
    /*
    private void HandleAbilities()
    {
        if (Input.GetKeyDown(KeyCode.Q)) // Cycle forward through abilities
        {
            currentAbility = (AbilityState)(((int)currentAbility + 1) % 3);
            ApplyCurrentAbility();
        }
        else if (Input.GetKeyDown(KeyCode.E)) // Reset to normal
        {
            currentAbility = AbilityState.Normal;
            ApplyCurrentAbility();
        }
    }

    public void ApplyCurrentAbility()
    {
        string abilityName = "";

        switch (currentAbility)
        {
            case AbilityState.Normal:
                jumpAngleX = 0.25f;
                jumpAngleY = 1f;
                colourMult = 1f;
                Physics2D.gravity = new Vector2(0, -9.8f);
                abilityName = "Vertical Jump";
                break;

            case AbilityState.HorizontalAirControl:
                jumpAngleX = 0.75f;
                jumpAngleY = 0.5f;
                colourMult = 1f;
                Physics2D.gravity = new Vector2(0, -9.8f);
                abilityName = "Horizontal Jump";
                break;

            case AbilityState.LowGravity:
                jumpAngleX = 0.35f;
                jumpAngleY = 0.75f;
                colourMult = 0.7f;
                Physics2D.gravity = new Vector2(0, -5f);
                abilityName = "Low Gravity Jump";
                break;
        }

        // Update the text if the reference exists
        if (abilityText != null)
        {
            abilityText.text = abilityName;
        }
        else
        {
            // Try to find the text component if not assigned
            abilityText = GetComponentInChildren<Text>();
            if (abilityText != null)
            {
                abilityText.text = abilityName;
            }
        }

        Debug.Log(abilityName);
    }
    */
}