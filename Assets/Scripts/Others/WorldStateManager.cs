
using UnityEngine;

public class WorldStateManager : MonoBehaviour
{
    public static WorldStateManager Instance { get; private set; }

    private Rigidbody[] worldObjects;

    private Vector3[] savedPositions;
    private Quaternion[] savedRotations;
    private Vector3[] savedVelocities;
    private Vector3[] savedAngularVelocities;

    private bool[] savedKinematicStates;
    private bool[] savedGravityStates;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        // Include inactive room objects
        Rigidbody[] allRigidbodies =
            FindObjectsByType<Rigidbody>(
                FindObjectsInactive.Include,
                FindObjectsSortMode.None
            );

        // Exclude Player
        System.Collections.Generic.List<Rigidbody> objects =
            new System.Collections.Generic.List<Rigidbody>();

        foreach (Rigidbody rb in allRigidbodies)
        {
            if (!rb.CompareTag("Player"))
            {
                objects.Add(rb);
            }
        }

        worldObjects = objects.ToArray();

        savedPositions = new Vector3[worldObjects.Length];
        savedRotations = new Quaternion[worldObjects.Length];
        savedVelocities = new Vector3[worldObjects.Length];
        savedAngularVelocities = new Vector3[worldObjects.Length];

        savedKinematicStates = new bool[worldObjects.Length];
        savedGravityStates = new bool[worldObjects.Length];

        Debug.Log("World Objects Found: " + worldObjects.Length);
    }

    public void SaveWorldState()
    {
        for (int i = 0; i < worldObjects.Length; i++)
        {
            Rigidbody rb = worldObjects[i];

            if (rb == null)
                continue;

            savedPositions[i] = rb.position;
            savedRotations[i] = rb.rotation;

            savedVelocities[i] = rb.linearVelocity;
            savedAngularVelocities[i] = rb.angularVelocity;

            savedKinematicStates[i] = rb.isKinematic;
            savedGravityStates[i] = rb.useGravity;
        }

        Debug.Log("World State Saved!");
    }

    
public void RestoreWorldState()
    {
        for (int i = 0; i < worldObjects.Length; i++)
        {
            Rigidbody rb = worldObjects[i];

            if (rb == null)
                continue;

            rb.isKinematic = true;

            rb.position = savedPositions[i];
            rb.rotation = savedRotations[i];

            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            rb.useGravity = savedGravityStates[i];
        }

        Physics.SyncTransforms();

        for (int i = 0; i < worldObjects.Length; i++)
        {
            Rigidbody rb = worldObjects[i];

            if (rb == null)
                continue;

            rb.isKinematic = savedKinematicStates[i];
        }

        Physics.SyncTransforms();

        Debug.Log("World State Restored!");
    }

}
