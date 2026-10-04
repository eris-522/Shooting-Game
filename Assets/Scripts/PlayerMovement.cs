using UnityEngine;
using UnityEngine.InputSystem; // Feature: New Input System API integration

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    // Feature: Movement and jumping parameter controls
    [Header("Movement")]
    public float moveSpeed = 8f;
    public float gravity = -19.62f;
    public float jumpHeight = 1.5f;

    // Feature: Camera look sensitivity and reference
    [Header("Look Sensitivity")]
    public float mouseSensitivity = 15f; // Note: Mouse delta values are smaller in the new system
    public Transform cameraTransform;

    private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;
    private float xRotation = 0f;

    void Start()
    {
        // Cache the character controller component
        controller = GetComponent<CharacterController>();

        // Lock cursor to screen center and hide it
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // Ensure hardware devices are connected before reading
        if (Keyboard.current == null || Mouse.current == null) return;

        HandleMouseLook();
        HandleMovement();
    }

    // Functionality: Reads raw mouse delta without redundant delta-time scaling
    void HandleMouseLook()
    {
        // Feature: Scaled pixel-based look rotation
        Vector2 mouseDelta = Mouse.current.delta.ReadValue() * mouseSensitivity;

        // Invert and clamp vertical camera rotation between -90 and 90 degrees
        xRotation -= mouseDelta.y;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseDelta.x);
    }

    // Functionality: Evaluates WASD keyboard states and applies gravity/jump impulses
    void HandleMovement()
    {
        isGrounded = controller.isGrounded;

        // Reset downward velocity on ground to prevent gravity buildup
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        // Read standard WASD keys
        float x = 0f;
        float z = 0f;

        if (Keyboard.current.aKey.isPressed) x -= 1f;
        if (Keyboard.current.dKey.isPressed) x += 1f;
        if (Keyboard.current.wKey.isPressed) z += 1f;
        if (Keyboard.current.sKey.isPressed) z -= 1f;

        // Normalize direction vector to prevent faster diagonal movement
        Vector3 move = (transform.right * x + transform.forward * z).normalized;
        controller.Move(move * moveSpeed * Time.deltaTime);

        // Jump execution via Spacebar press on current frame
        if (Keyboard.current.spaceKey.wasPressedThisFrame && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        // Apply constant downward acceleration over time
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}