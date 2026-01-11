using UnityEngine;

public class WindIndicator : MonoBehaviour
{
    [Header("References")]
    public ShipController ship;
    public RectTransform arrow;

    [Header("Display")]
    public float arrowZeroOffsetDeg = 20f;
    public bool showWindComingFrom = false;
    public bool relativeToShip = true;

    void Update()
    {
        if (ship == null || arrow == null) return;

        float windDeg = ship.windDirDeg;

        // Flip direction to show where wind comes from instead of where it goes
        if (showWindComingFrom)
            windDeg += 180f;

        float displayDeg = windDeg;

        // Convert wind direction to ship-relative space
        if (relativeToShip)
            displayDeg -= ship.headingDeg;

        // Apply UI rotation with sprite alignment offset
        arrow.localEulerAngles = new Vector3(0f, 0f, displayDeg + arrowZeroOffsetDeg);
    }
}
