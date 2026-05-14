using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;

    [Header("Jump")]
    [SerializeField] private float gravity = -20f;
    [SerializeField] private float jumpHeight = 1.5f;
    [SerializeField] private float maxJumpHoldTime = 0.25f;
    [SerializeField] private float jumpHoldForce = 12f;

    [Header("Jump Forgiveness")]
    [SerializeField] private float coyoteTime = 0.15f;
    [SerializeField] private float jumpBufferTime = 0.15f;

    [Header("References")]
    [SerializeField] private CharacterController characterController;

    private Vector3 velocity;

    private float coyoteTimer;
    private float jumpBufferTimer;
    private float jumpHoldTimer;

    private bool isHoldingJump;

    private void Reset()
    {
        characterController = GetComponent<CharacterController>();
    }

    private void Start()
    {
        if (characterController == null)
        {
            characterController = GetComponent<CharacterController>();
        }
    }

    private void Update()
    {
        UpdateGroundedState();
        ReadJumpInput();
        MovePlayer();
        HandleJump();
        ApplyGravity();
    }

    private void UpdateGroundedState()
    {
        if (characterController.isGrounded)
        {
            coyoteTimer = coyoteTime;

            if (velocity.y < 0f)
            {
                velocity.y = -2f;
            }
        }
        else
        {
            coyoteTimer -= Time.deltaTime;
        }
    }

    private void ReadJumpInput()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            jumpBufferTimer = jumpBufferTime;
        }
        else
        {
            jumpBufferTimer -= Time.deltaTime;
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            isHoldingJump = false;
        }
    }

    private void MovePlayer()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;
        characterController.Move(move * moveSpeed * Time.deltaTime);
    }

    private void HandleJump()
    {
        bool canJump = coyoteTimer > 0f;
        bool wantsToJump = jumpBufferTimer > 0f;

        if (wantsToJump && canJump)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);

            jumpBufferTimer = 0f;
            coyoteTimer = 0f;

            isHoldingJump = true;
            jumpHoldTimer = 0f;
        }

        if (Input.GetKey(KeyCode.Space) && isHoldingJump)
        {
            if (jumpHoldTimer < maxJumpHoldTime)
            {
                velocity.y += jumpHoldForce * Time.deltaTime;
                jumpHoldTimer += Time.deltaTime;
            }
            else
            {
                isHoldingJump = false;
            }
        }
    }

    private void ApplyGravity()
    {
        velocity.y += gravity * Time.deltaTime;
        characterController.Move(velocity * Time.deltaTime);
    }
}