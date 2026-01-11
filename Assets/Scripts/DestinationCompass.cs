using UnityEngine;

public class DestinationCompass : MonoBehaviour
{
    public Transform destination;

    [Header("Settings")]
    public float rotationOffset = 0f;

    void Update()
    {
        if (destination == null) return;

        // Destination world position is used as direction from ship origin
        Vector3 dir = destination.position;

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle + rotationOffset);
    }
}
