using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

/// <summary>
/// First-person movement and look, driven entirely by the new Input System.
/// Works with keyboard + mouse, gamepad, and the on-screen sticks used on mobile.
/// </summary>
[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] private InputActionAsset controls;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 3.5f;
    [SerializeField] private float gravity = -9.81f;

    [Header("Look")]
    [SerializeField] private Transform cameraPivot;
    [SerializeField] private float mouseSensitivity = 0.1f;   // degrees per mouse pixel
    [SerializeField] private float stickSensitivity = 120f;   // degrees per second at full tilt
    [SerializeField] private float maxPitch = 80f;

    private CharacterController characterController;
    private InputActionMap playerMap;
    private InputAction moveAction;
    private InputAction lookAction;
    private InputAction lookStickAction;

    private float pitch;
    private float verticalVelocity;
    private bool controlEnabled = true;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();

        playerMap = controls.FindActionMap("Player", true);
        moveAction = playerMap.FindAction("Move", true);
        lookAction = playerMap.FindAction("Look", true);
        lookStickAction = playerMap.FindAction("LookStick", true);
    }

    private void OnEnable() => playerMap.Enable();

    private void OnDisable() => playerMap.Disable();

    private void Start() => SetControlEnabled(true);

    private void Update()
    {
        RelockCursorOnClick();

        if (controlEnabled)
        {
            Look();
        }

        Move();
    }

    /// <summary>Called by UI (e.g. the story panel) to pause or resume player control.</summary>
    public void SetControlEnabled(bool value)
    {
        controlEnabled = value;
        Cursor.lockState = value ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !value;
    }

    private void Look()
    {
        Vector2 look = lookStickAction.ReadValue<Vector2>() * stickSensitivity * Time.deltaTime;

        // Mouse delta is only used while the cursor is captured.
        if (Cursor.lockState == CursorLockMode.Locked)
        {
            look += lookAction.ReadValue<Vector2>() * mouseSensitivity;
        }

        transform.Rotate(Vector3.up * look.x);
        pitch = Mathf.Clamp(pitch - look.y, -maxPitch, maxPitch);
        cameraPivot.localRotation = Quaternion.Euler(pitch, 0f, 0f);
    }

    private void Move()
    {
        Vector2 input = controlEnabled ? moveAction.ReadValue<Vector2>() : Vector2.zero;
        Vector3 horizontal = (transform.right * input.x + transform.forward * input.y) * moveSpeed;

        if (characterController.isGrounded && verticalVelocity < 0f)
        {
            verticalVelocity = -2f;
        }
        verticalVelocity += gravity * Time.deltaTime;

        Vector3 velocity = horizontal + Vector3.up * verticalVelocity;
        characterController.Move(velocity * Time.deltaTime);
    }

    // Browsers (WebGL) and the editor release the cursor on Esc; a click recaptures it.
    private void RelockCursorOnClick()
    {
        bool overUI = EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();

        if (controlEnabled
            && !overUI
            && Cursor.lockState != CursorLockMode.Locked
            && Mouse.current != null
            && Mouse.current.leftButton.wasPressedThisFrame)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}
