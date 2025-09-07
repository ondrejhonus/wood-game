using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed;

    public float groundDrag;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask whatIsGround;
    public bool grounded;

    public Transform orientation;

    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;

    Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true; // dont allow the player to flip over
    }
    private void InputHandler()
    {
        var keyboard = Keyboard.current;
        horizontalInput = 0f;
        verticalInput = 0f;

        // keyboard input
        if (keyboard != null)
        {
            if (keyboard.aKey.isPressed) horizontalInput -= 1f;
            if (keyboard.dKey.isPressed) horizontalInput += 1f;
            if (keyboard.wKey.isPressed) verticalInput += 1f;
            if (keyboard.sKey.isPressed) verticalInput -= 1f;
        }
    }

    private void MovePlayer()
    {
        // calculate the move direction based on input and orientation
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
    }

    private void Update()
    {
        // check if player is grounded
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);

        InputHandler();

        // apply drag when grounded
        if (grounded)
        {
            rb.linearDamping = groundDrag;
            Debug.Log("Grounded");
        }
        else
        {
            rb.linearDamping = 0f; // no drag when in the air
            Debug.Log("Not Grounded");
        }
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }
}