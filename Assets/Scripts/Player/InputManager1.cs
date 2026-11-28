
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager1 : MonoBehaviour
{
    InputActions inputActions;
    PlayerLook look;

    public Vector2 moveInput;
    public Vector2 lookInput;
    public bool jumpInput;

    void Awake()
    {
        inputActions = new InputActions();
        look = GetComponent<PlayerLook>();
    }

    void Update()
    {
        moveInput = inputActions.Player.Move.ReadValue<Vector2>();
        lookInput = inputActions.Player.Look.ReadValue<Vector2>();
        jumpInput = inputActions.Player.Jump.WasPressedThisFrame();
    }

    void LateUpdate()
    {
        if (look == null)
            return;

        Vector2 mouseInput = Mouse.current != null
            ? Mouse.current.delta.ReadValue()
            : Vector2.zero;

        Vector2 joystickInput = lookInput * (Time.deltaTime * 1000f);

        Vector2 finalInput = mouseInput + joystickInput;

        look.ProcessLook(finalInput);
    }

    void OnEnable()
    {
        inputActions.Enable();
    }

    void OnDisable()
    {
        inputActions.Disable();
    }
}
