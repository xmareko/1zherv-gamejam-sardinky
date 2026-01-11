using UnityEngine;

public class SailVisual : MonoBehaviour
{
    public ShipController ship;

    [Header("Rotation")]
    public float maxVisualAngle = 60f;
    public float smooth = 12f;

    [Header("Axis")]
    public bool rotateLocal = true;
    public float baseAngleOffset = 0f;

    float current;

    void Update()
    {
        if (ship == null) return;

        // Convert sail trim (-1..1) to visual angle
        float target = ship.sailTrim * maxVisualAngle + baseAngleOffset;

        current = Mathf.Lerp(
            current,
            target,
            1f - Mathf.Exp(-smooth * Time.deltaTime)
        );

        if (rotateLocal)
            transform.localEulerAngles = new Vector3(0, 0, current);
        else
            transform.eulerAngles = new Vector3(0, 0, current);
    }
}
