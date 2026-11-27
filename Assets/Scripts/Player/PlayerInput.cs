
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
    InputActions inputActions;
    Rigidbody rb;

    public float speed = 5f;
    public float smoothTime = 0.2f;
    public float jumpForce = 5f;
    public float fallSpeed = 15f;

    public bool canMove = true;

    Vector3 moveDirection;
    Transform cam;

    float turnVelocity;
    int jumpCount;

    void Awake()
    {
        inputActions = new InputActions();
        rb = GetComponent<Rigidbody>();
        cam = Camera.main.transform;
    }

    void Update()
    {
        if (!canMove)
        {
            moveDirection = Vector3.zero;
            return;
        }

        Vector2 input = inputActions.Player.Move.ReadValue<Vector2>();

        Vector3 forward = cam.forward;
        Vector3 right = cam.right;

        forward.y = 0f;
        right.y = 0f;

        forward.Normalize();
        right.Normalize();

        moveDirection = forward * input.y + right * input.x;

        if (inputActions.Player.Jump.WasPressedThisFrame() && jumpCount < 2)
        {
            rb.linearVelocity = new Vector3(
                rb.linearVelocity.x,
                0f,
                rb.linearVelocity.z
            );

            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            jumpCount++;
        }
    }

    void FixedUpdate()
    {
        if (!canMove)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            return;
        }

        rb.linearVelocity = new Vector3(
            moveDirection.x * speed,
            rb.linearVelocity.y,
            moveDirection.z * speed
        );

        if (rb.linearVelocity.y < 0f)
        {
            rb.AddForce(Vector3.down * fallSpeed, ForceMode.Acceleration);
        }

        if (moveDirection != Vector3.zero)
        {
            float targetAngle = Mathf.Atan2(
                moveDirection.x,
                moveDirection.z
            ) * Mathf.Rad2Deg;

            float smoothAngle = Mathf.SmoothDampAngle(
                rb.rotation.eulerAngles.y,
                targetAngle,
                ref turnVelocity,
                smoothTime
            );

            rb.MoveRotation(
                Quaternion.Euler(0f, smoothAngle, 0f)
            );
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            jumpCount = 0;
        }
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
