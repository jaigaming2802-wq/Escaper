
using UnityEngine;
using System;

public class ObjectFallDetection : MonoBehaviour
{
    public static event Action<Vector3, Vector3> OnObjectFallen;

    bool hasFallen = false;
    Vector3 originalPosition;

    void Start()
    {
        originalPosition = transform.position;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (hasFallen)
            return;

        if (collision.gameObject.CompareTag("Floor"))
        {
            hasFallen = true;

            Vector3 fallenPosition = transform.position;

            OnObjectFallen?.Invoke(originalPosition, fallenPosition);

            Debug.Log(gameObject.name + " fell on the floor!");
        }
    }
}
