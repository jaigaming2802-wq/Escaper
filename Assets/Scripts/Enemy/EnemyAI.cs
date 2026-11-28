
using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class EnemyAI : MonoBehaviour
{
    NavMeshAgent agent;
    Animator animator;

    public Transform enemyStartPoint;

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

        while (agent.pathPending || agent.remainingDistance > agent.stoppingDistance)
        {
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
        {
            yield return null;
        }

        // First Search animation
        while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)
        {
            if (playerDetected)
                yield break;

            yield return null;
        }

        if (playerDetected)
            yield break;

        // Second Search animation
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
        agent.SetDestination(enemyStartPoint.position);

        while (agent.pathPending || agent.remainingDistance > agent.stoppingDistance)
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
        Collider[] players = Physics.OverlapSphere(
            transform.position,
            detectionRadius,
            playerLayer
        );

        foreach (Collider player in players)
        {
            Vector3 origin = eyePoint != null
                ? eyePoint.position
                : transform.position + Vector3.up * 1.5f;

            Vector3 target = player.bounds.center;
            Vector3 direction = target - origin;

            Debug.DrawRay(origin, direction, Color.yellow);

            if (Physics.Raycast(
                origin,
                direction.normalized,
                out RaycastHit hit,
                direction.magnitude,
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

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
