using UnityEngine;

public class SteeringVisual : MonoBehaviour
{
    [Header("References")]
    public ShipController ship;

    [Header("Rotation")]
    public float maxAngle = 40f;        // max natočení ploutve ve stupních
    public float smooth = 12f;          // plynulost dorovnání
    public bool rotateLocal = true;     // doporučeno true
    public float baseAngleOffset = 0f;  // doladění, když je sprite defaultně natočený

    [Header("Behavior")]
    public bool invert = false;          // true = helm doprava => ploutev doleva (realistické)

    float current;

    void Update()
    {
        if (ship == null) return;

        // normalizuj helm na -1..+1 (nezávisle na helmMax)
        float t = 0f;
        if (ship.helmMax > 0.0001f)
            t = ship.helm / ship.helmMax;

        t = Mathf.Clamp(t, -1f, 1f);

        // realisticky: když loď zatáčí doprava (helm +), ploutev jde doleva
        float sign = invert ? -1f : 1f;
        float target = (sign * t * maxAngle) + baseAngleOffset;

        // plynulé dorovnání (stabilní i při různém FPS)
        current = Mathf.Lerp(current, target, 1f - Mathf.Exp(-smooth * Time.deltaTime));

        if (rotateLocal)
            transform.localEulerAngles = new Vector3(0f, 0f, current);
        else
            transform.eulerAngles = new Vector3(0f, 0f, current);
    }
}