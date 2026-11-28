
using UnityEngine;
using System.Collections;

public class OutCollision : MonoBehaviour
{
    private bool isTriggered;

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Player") || isTriggered)
            return;

        isTriggered = true;

        PlayerInput playerInput =
            collision.gameObject.GetComponent<PlayerInput>();

        Rigidbody rb = collision.gameObject.GetComponent<Rigidbody>();

        if (playerInput != null)
            playerInput.canMove = false;

        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
        }

        StartCoroutine(Respawn(playerInput, rb));
    }

    IEnumerator Respawn(PlayerInput playerInput, Rigidbody rb)
    {
        yield return new WaitForSeconds(3f);

        CheckpointManager.Instance.RespawnPlayer();

        if (rb != null)
        {
            rb.isKinematic = false;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        if (playerInput != null)
            playerInput.canMove = true;

        isTriggered = false;
    }
}
