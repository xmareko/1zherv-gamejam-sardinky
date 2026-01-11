using UnityEngine;

public class DestinationSpawner : MonoBehaviour
{
    [Header("Settings")]
    public Transform destinationPort; // The object to move (DestinationPort)
    public Transform compass;         // The Compass object (optional, if you need to auto-link it)

    [Header("Possible Locations")]
    public Vector3[] spawnPoints; // Define coordinates manually in Inspector

    void Start()
    {
        if (destinationPort == null)
        {
            Debug.LogError("DestinationPort is not assigned!");
            return;
        }

        if (spawnPoints.Length > 0)
        {
            // Pick a random index
            int index = Random.Range(0, spawnPoints.Length);

            // Move the port to that position
            destinationPort.position = spawnPoints[index];

            Debug.Log($"Destination spawned at index {index}: {spawnPoints[index]}");
        }
        else
        {
            Debug.LogWarning("No spawn points defined in DestinationSpawner.");
        }

        // Optional: Update compass reference if it was lost (usually not needed if reference is direct)
        // If your compass script needs to know the NEW transform, it already has it since we moved the same object.
    }
}