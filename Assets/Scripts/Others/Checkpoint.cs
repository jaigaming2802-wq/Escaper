
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
            Transform cameraTransform = Camera.main.transform;

            CheckpointManager.Instance.SetCheckpoint(
                checkpointCollider.bounds.center,
                cameraTransform.eulerAngles.y
            );
        }
    }
}
