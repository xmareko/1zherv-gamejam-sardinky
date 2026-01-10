using UnityEngine;

public class DestinationCompass : MonoBehaviour
{
    [Header("References")]
    public Transform destinationTarget; // Drag "DestinationPort" here from Hierarchy

    [Header("Settings")]
    public float angleOffset = 0f;      // Change this to -90 or 90 if arrow points wrong way

    void Update()
    {
        if (destinationTarget == null) return;

        // 1. Get position of target. Since our ship is fixed at (0,0),
        // the target's position IS the direction vector.
        Vector3 dir = destinationTarget.position;

        // 2. Calculate angle (Atan2 returns radians, convert to degrees)
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        // 3. Rotate THIS object (the UI Image)
        // We add angleOffset to fix sprite rotation (e.g. if sprite points Up, use -90)
        transform.rotation = Quaternion.Euler(0, 0, angle + angleOffset);
    }
}