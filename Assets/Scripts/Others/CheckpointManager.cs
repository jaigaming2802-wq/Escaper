
using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    public static CheckpointManager Instance { get; private set; }

    [SerializeField] private Transform startingRespawnPoint;

    private Vector3 lastCheckpointPosition;

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
        }
    }

    public void SetCheckpoint(Vector3 position)
    {
        lastCheckpointPosition = position;
        Debug.Log("Checkpoint Saved: " + position);
    }

    public void RespawnPlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player == null)
            return;

        Rigidbody rb = player.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.position = lastCheckpointPosition;
        }
        else
        {
            player.transform.position = lastCheckpointPosition;
        }

        Debug.Log("Player Respawned!");
    }
}
