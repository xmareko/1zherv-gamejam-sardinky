using UnityEngine;

public class CompassUI : MonoBehaviour
{
    public ShipController ship;

    [Header("Settings")]
    public float angleOffset = 0f; // Use this to align 'N' correctly

    void Update()
    {
        if (ship == null) return;

        // The world rotates opposite to the ship's heading.
        // We rotate the compass so 'North' stays fixed in world space.
        float rotation = -ship.headingDeg + angleOffset;

        transform.rotation = Quaternion.Euler(0, 0, rotation);
    }
}