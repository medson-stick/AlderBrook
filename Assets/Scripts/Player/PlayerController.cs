using UnityEngine;
using UnityEngine.InputSystem;

// Moves the player in a top-down world.
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(PlayerInput))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 4f;
    [SerializeField] private SpriteRenderer spriteRenderer;

    private Rigidbody2D rb;
    private PlayerInput playerInput;
    private Vector2 moveInput;

    public Vector2 FacingDirection { get; private set; } = Vector2.down;

    // Caches the components and sets up physics.
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        playerInput = GetComponent<PlayerInput>();
        if (spriteRenderer == null) spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        rb.gravityScale = 0f;
        rb.freezeRotation = true;
    }

    // Reads movement input from the Input System.
    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    // Applies movement and updates facing.
    private void FixedUpdate()
    {
        Vector2 movement = moveInput.normalized;
        rb.linearVelocity = movement * moveSpeed;

        if (movement.sqrMagnitude > 0.01f)
        {
            FacingDirection = movement;
            if (spriteRenderer != null) spriteRenderer.flipX = movement.x < -0.01f;
        }
    }

    // Enables or disables player input.
    public void SetInputEnabled(bool isEnabled)
    {
        if (playerInput != null) playerInput.enabled = isEnabled;
        if (!isEnabled && rb != null) rb.linearVelocity = Vector2.zero;
        moveInput = Vector2.zero;
    }

    // Stops the player when the script is disabled.
    private void OnDisable()
    {
        if (rb != null) rb.linearVelocity = Vector2.zero;
    }
}
