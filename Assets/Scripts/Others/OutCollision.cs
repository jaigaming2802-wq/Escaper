
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

        PlayerInput playerInput = collision.gameObject.GetComponent<PlayerInput>();

        if (playerInput != null)
        {
            playerInput.canMove = false;
        }

        StartCoroutine(Respawn(playerInput));
    }

    IEnumerator Respawn(PlayerInput playerInput)
    {
        yield return new WaitForSeconds(3f);

        CheckpointManager.Instance.RespawnPlayer();

        if (playerInput != null)
        {
            playerInput.canMove = true;
        }

        isTriggered = false;
    }
}
