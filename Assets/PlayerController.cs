using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float speed = 5f;
    [SerializeField] private float jumpForce = 10f;

    [Header("Ground Check Settings")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float checkRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float slopeRayDistance = 1f;

    [Header("References")]
    [SerializeField] private Rigidbody2D rb2D;
    [SerializeField] private Transform spawnPos;

    private float _horizontalInput;
    private bool _jumpRequested;
    private bool _isGrounded;

    void Start()
    {
        if (spawnPos != null)
        {
            transform.position = spawnPos.position;
        }
    }

    void Update()
    {
        // 1. Collect inputs in Update
        _horizontalInput = 0f;

        if (Keyboard.current != null)
        {
            if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) 
                _horizontalInput -= 1f;
            if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) 
                _horizontalInput += 1f;

            if (Keyboard.current.spaceKey.wasPressedThisFrame)
            {
                _jumpRequested = true;
            }
        }
    }

    void FixedUpdate()
    {
        // 2. Perform physics checks in FixedUpdate
        _isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, groundLayer);

        // 3. Align body to slope normal using 2D Raycast
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, slopeRayDistance, groundLayer);
        if (hit.collider)
        {
            transform.up = hit.normal;
        }

        // 4. Apply horizontal velocity
        rb2D.linearVelocity = new Vector2(_horizontalInput * speed, rb2D.linearVelocity.y);

        // 5. Apply jump impulse
        if (_jumpRequested && _isGrounded)
        {
            rb2D.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }

        _jumpRequested = false; // Reset jump flag after physics tick
    }

    private void OnDrawGizmosSelected()
    {
        // Draw the ground check radius in the editor for easy visual debugging
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, checkRadius);
        }
    }
}