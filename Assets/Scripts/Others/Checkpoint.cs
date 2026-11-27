using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        Collider checkpointCollider = GetComponent<Collider>();

        if (checkpointCollider != null)
        {
            CheckpointManager.Instance.SetCheckpoint(
                checkpointCollider.bounds.center
            );
        }
    }
}