using UnityEngine;

public class SailVisual : MonoBehaviour
{
    public ShipController ship;

    [Header("Rotation")]
    public float maxVisualAngle = 60f;   // kolik stupňů max se plachta otočí
    public float smooth = 12f;           // jak plynule se to dorovnává

    [Header("Axis")]
    public bool rotateLocal = true;      // doporučeno true
    public float baseAngleOffset = 0f;   // pokud je sprite defaultně nakřivo, doladíš

    float current;

    void Update()
    {
        if (ship == null) return;

        // sailTrim je -1..+1 -> úhel -max..+max
        float target = ship.sailTrim * maxVisualAngle + baseAngleOffset;

        // plynulé otáčení
        current = Mathf.Lerp(current, target, 1f - Mathf.Exp(-smooth * Time.deltaTime));

        if (rotateLocal)
            transform.localEulerAngles = new Vector3(0, 0, current);
        else
            transform.eulerAngles = new Vector3(0, 0, current);
    }
}