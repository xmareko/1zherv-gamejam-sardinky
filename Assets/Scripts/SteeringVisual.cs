using UnityEngine;

public class SteeringVisual : MonoBehaviour
{
    [Header("References")]
    public ShipController ship;

    [Header("Rotation")]
    public float maxAngle = 40f;
    public float smooth = 12f;
    public bool rotateLocal = true;
    public float baseAngleOffset = 0f;

    [Header("Behavior")]
    public bool invert = false;

    float current;

    void Update()
    {
        if (ship == null) return;

        // Normalize helm to -1..+1 regardless of helmMax
        float t = 0f;
        if (ship.helmMax > 0.0001f)
            t = ship.helm / ship.helmMax;

        t = Mathf.Clamp(t, -1f, 1f);

        // Optionally invert for "rudder opposite to turn" visuals
        float sign = invert ? -1f : 1f;
        float target = (sign * t * maxAngle) + baseAngleOffset;

        // Framerate-independent smoothing
        current = Mathf.Lerp(current, target, 1f - Mathf.Exp(-smooth * Time.deltaTime));

        if (rotateLocal)
            transform.localEulerAngles = new Vector3(0f, 0f, current);
        else
            transform.eulerAngles = new Vector3(0f, 0f, current);
    }
}
