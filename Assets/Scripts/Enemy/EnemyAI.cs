
using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class EnemyAI : MonoBehaviour
{
    NavMeshAgent agent;
    Animator animator;

    public Transform enemyStartPoint;

    public DoorController doorController;
    public Transform doorPoint;
    public Transform doorExitPoint;

    public float doorOpenDistance = 2f;
    public float doorOpenWaitTime = 1f;

    public float detectionRadius = 5f;
    public LayerMask playerLayer;
    public Transform eyePoint;

    bool isSearching = false;
    bool playerDetected = false;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }

    void OnEnable()
    {
        ObjectFallDetection.OnObjectFallen += Investigate;
    }

    void OnDisable()
    {
        ObjectFallDetection.OnObjectFallen -= Investigate;
    }

    void Investigate(Vector3 originalPosition, Vector3 fallenPosition)
    {
        if (isSearching)
            return;

        StartCoroutine(SearchAndReturn(originalPosition, fallenPosition));
    }

    IEnumerator SearchAndReturn(Vector3 originalPosition, Vector3 fallenPosition)
    {
        isSearching = true;
        playerDetected = false;

        animator.speed = 1f;

        agent.isStopped = false;
        agent.SetDestination(fallenPosition);

        while (agent.pathPending)
            yield return null;

        bool doorOpened = false;

        while (agent.pathPending || agent.remainingDistance > agent.stoppingDistance)
        {
            if (!doorOpened && doorPoint != null && doorController != null)
            {
                float distance = Vector3.Distance(
                    transform.position,
                    doorPoint.position
                );

                if (distance <= doorOpenDistance)
                {
                    agent.isStopped = true;

                    Debug.Log("ENEMY REACHED DOOR POINT - OPENING DOOR");

                    doorController.OpenDoor();

                    yield return new WaitForSeconds(doorOpenWaitTime);

                    doorOpened = true;

                    agent.isStopped = false;
                    agent.SetDestination(fallenPosition);
                }
            }

            yield return null;
        }

        agent.isStopped = true;
        agent.updateRotation = false;

        Vector3 direction = originalPosition - transform.position;
        direction.y = 0f;

        if (direction.sqrMagnitude > 0.01f)
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }

        animator.SetBool("IsSearching", true);

        while (!animator.GetCurrentAnimatorStateInfo(0).IsName("Search"))
            yield return null;

        while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)
        {
            if (playerDetected)
                yield break;

            yield return null;
        }

        if (playerDetected)
            yield break;

        animator.Play("Search", 0, 0f);

        yield return null;

        while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)
        {
            if (playerDetected)
                yield break;

            yield return null;
        }

        if (playerDetected)
            yield break;

        animator.SetBool("IsSearching", false);

        agent.updateRotation = true;
        agent.isStopped = false;

        // Go to Door Exit Point
        if (doorOpened && doorExitPoint != null)
        {
            Debug.Log("EXIT POINT TARGET: " + doorExitPoint.position);

            agent.SetDestination(doorExitPoint.position);

            while (agent.pathPending)
                yield return null;

            Debug.Log("EXIT PATH STATUS: " + agent.pathStatus);

            while (agent.remainingDistance > agent.stoppingDistance)
            {
                float distance = Vector3.Distance(
                    transform.position,
                    doorExitPoint.position
                );

                Debug.Log("ENEMY TO EXIT POINT DISTANCE: " + distance);

                yield return null;
            }

            Debug.Log("ENEMY REACHED EXIT POINT!");

            agent.isStopped = true;

            Debug.Log("CALLING CLOSE DOOR!");

            doorController.CloseDoor();

            Debug.Log("CLOSE DOOR FUNCTION CALLED!");

            doorOpened = false;
        }
        else
        {
            Debug.LogWarning("DOOR NOT OPENED OR DOOR EXIT POINT NOT ASSIGNED!");
        }

        // Return to starting position
        agent.isStopped = false;
        agent.SetDestination(enemyStartPoint.position);

        while (agent.pathPending ||
               agent.remainingDistance > agent.stoppingDistance)
        {
            yield return null;
        }

        agent.isStopped = true;
        isSearching = false;
    }

    void Update()
    {
        animator.SetFloat("Speed", agent.velocity.magnitude);

        if (isSearching && !playerDetected)
        {
            CheckPlayer();
        }
    }

    void CheckPlayer()
    {
        Vector3 origin = eyePoint != null
            ? eyePoint.position
            : transform.position + Vector3.up * 1.5f;

        int rayCount = 36;

        for (int i = 0; i < rayCount; i++)
        {
            float horizontalAngle = i * 10f;

            for (int j = -2; j <= 2; j++)
            {
                float verticalAngle = j * 20f;

                Vector3 direction =
                    Quaternion.Euler(verticalAngle, horizontalAngle, 0f)
                    * transform.forward;

                Debug.DrawRay(
                    origin,
                    direction * detectionRadius,
                    Color.yellow
                );

                if (Physics.Raycast(
                    origin,
                    direction,
                    out RaycastHit hit,
                    detectionRadius,
                    Physics.AllLayers,
                    QueryTriggerInteraction.Ignore))
                {
                    if (hit.collider.CompareTag("Player") ||
                        hit.collider.transform.root.CompareTag("Player"))
                    {
                        playerDetected = true;
                        animator.speed = 0f;

                        Debug.Log("PLAYER DETECTED!");

                        return;
                    }
                }
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
