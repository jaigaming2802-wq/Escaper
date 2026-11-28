
using UnityEngine;
using System.Collections;

public class DoorController : MonoBehaviour
{
    Animator animator;

    Coroutine doorCoroutine;

    public float animationDuration = 1f;

    void Awake()
    {
        animator = GetComponent<Animator>();

        animator.Play("Door", 0, 0f);
        animator.Update(0f);
        animator.speed = 0f;
    }

    public void OpenDoor()
    {
        if (doorCoroutine != null)
            StopCoroutine(doorCoroutine);

        doorCoroutine = StartCoroutine(AnimateDoor(0f, 1f));
    }

    public void CloseDoor()
    {
        Debug.Log("DOOR CLOSE STARTED");

        if (doorCoroutine != null)
            StopCoroutine(doorCoroutine);

        doorCoroutine = StartCoroutine(AnimateDoor(1f, 0f));
    }

    IEnumerator AnimateDoor(float startValue, float endValue)
    {
        float timer = 0f;

        animator.speed = 0f;

        while (timer < animationDuration)
        {
            timer += Time.deltaTime;

            float progress = Mathf.Clamp01(timer / animationDuration);

            float currentTime = Mathf.Lerp(
                startValue,
                endValue,
                progress
            );

            animator.Play("Door", 0, currentTime);
            animator.Update(0f);

            yield return null;
        }

        animator.Play("Door", 0, endValue);
        animator.Update(0f);

        animator.speed = 0f;

        if (endValue == 0f)
            Debug.Log("DOOR CLOSED COMPLETELY");
        else
            Debug.Log("DOOR OPENED COMPLETELY");
    }
}
