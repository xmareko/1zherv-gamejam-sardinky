using UnityEngine;

public class WindIndicator : MonoBehaviour
{
    [Header("References")]
    public ShipController ship;            // ShipRoot se ShipControllerem
    public RectTransform arrow;            // UI šipka (WindArrow)

    [Header("Display")]
    public float arrowZeroOffsetDeg = 20f; // doladění orientace sprite (např. -90, 0, 90, 180)
    public bool showWindComingFrom = false; // true = odkud fouká, false = kam fouká
    public bool relativeToShip = true;     // true = relativně k lodi (doporučeno), false = absolutně

    void Update()
    {
        if (ship == null || arrow == null) return;

        float windDeg = ship.windDirDeg;

        // Pokud chceme "odkud fouká", otočíme směr o 180°
        if (showWindComingFrom)
            windDeg = windDeg + 180f;

        // Pokud chceme relativně k lodi, odečteme kurz lodi
        // (0° na UI bude "dopředu" lodi, u tebe je to doprava na obrazovce)
        float displayDeg = windDeg;
        if (relativeToShip)
            displayDeg = windDeg - ship.headingDeg;

        // UI rotace: v UI je kladná rotace kolem Z proti směru hodin.
        // -displayDeg obvykle sedí jako "pravá = 0°"
        arrow.localEulerAngles = new Vector3(0f, 0f, displayDeg + arrowZeroOffsetDeg);
    }
}