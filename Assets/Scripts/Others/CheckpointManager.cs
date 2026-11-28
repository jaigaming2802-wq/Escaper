
using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    public static CheckpointManager Instance { get; private set; }

    [SerializeField] private Transform startingRespawnPoint;

    private Vector3 lastCheckpointPosition;
    private float lastCameraRotation;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        if (startingRespawnPoint != null)
        {
            lastCheckpointPosition = startingRespawnPoint.position;
            lastCameraRotation = startingRespawnPoint.eulerAngles.y;
        }
    }

    private void Start()
    {
        // Save initial environment state
        WorldStateManager.Instance.SaveWorldState();
    }

    public void SetCheckpoint(Vector3 position, float cameraRotation)
    {
        lastCheckpointPosition = position;
        lastCameraRotation = cameraRotation;

        // Save environment at checkpoint
        WorldStateManager.Instance.SaveWorldState();

        Debug.Log("Checkpoint Saved: " + position);
    }

    public void RespawnPlayer()
    {
        // Restore environment first
        WorldStateManager.Instance.RestoreWorldState();

        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player == null)
            return;

        Rigidbody rb = player.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            rb.position = lastCheckpointPosition;
            rb.rotation = Quaternion.Euler(0f, lastCameraRotation, 0f);
        }
        else
        {
            player.transform.position = lastCheckpointPosition;
            player.transform.rotation =
                Quaternion.Euler(0f, lastCameraRotation, 0f);
        }

        Debug.Log("Player Respawned!");
    }
}
