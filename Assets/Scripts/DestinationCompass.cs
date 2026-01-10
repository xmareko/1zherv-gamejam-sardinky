using UnityEngine;

public class DestinationCompass : MonoBehaviour
{
    public Transform destination; // Drag DestinationPort here

    [Header("Settings")]
    public float rotationOffset = 0f; // Adjust if arrow points wrong (usually 0, -90, or 90)

    void Update()
    {
        if (destination == null) return;

        // 1. Get position of the target
        // Since the Ship is always fixed at (0,0), the target's world position 
        // IS the direction vector from the ship.
        Vector3 dir = destination.position;

        // 2. Calculate the angle in degrees
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        // 3. Apply rotation to THIS object
        transform.rotation = Quaternion.Euler(0, 0, angle + rotationOffset);
    }
}