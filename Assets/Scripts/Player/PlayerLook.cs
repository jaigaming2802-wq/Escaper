
using UnityEngine;

public class PlayerLook : MonoBehaviour
{
    public Transform target;

    public float sensitivity = 0.1f;
    public float minPitch = -20f;
    public float maxPitch = 60f;

    private float yaw;
    private float pitch;
    private Vector3 offset;

    void Start()
    {
        if (target == null)
        {
            enabled = false;
            return;
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        yaw = transform.eulerAngles.y;
        pitch = transform.eulerAngles.x;

        if (pitch > 180f)
            pitch -= 360f;

        offset = Quaternion.Inverse(transform.rotation) *
                 (transform.position - target.position);
    }

    public void ProcessLook(Vector2 input)
    {
        yaw += input.x * sensitivity;
        pitch -= input.y * sensitivity;

        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);

        transform.rotation = rotation;
        transform.position = target.position + rotation * offset;
    }
}
