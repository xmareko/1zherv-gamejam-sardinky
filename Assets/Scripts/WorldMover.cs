using UnityEngine;
using UnityEngine.InputSystem;

public class WorldMover : MonoBehaviour
{
    [Header("Sailing")]
    public float maxSpeed = 8f;
    public float accel = 2.5f;      // jak rychle loď zrychluje/brzdí
    public float noGoZoneDeg = 35f; // úhel proti větru, kde skoro nejede

    
    [Header("References")]
    public ShipController ship;   // ShipRoot s ShipControllerem

    [Header("Movement")]
    public float forwardSpeed = 5f;     // když nechceš brát ship.speed, můžeš použít tohle
    public bool useShipSpeed = true;    // true = používá ship.speed

    [Header("Turning")]
    public float turnSpeed = 120f;      // deg/sec

    // Pivot: střed lodi (loď je fixně na (0,0,0))
    Vector3 pivotPoint = Vector3.zero;

    void Update()
    {
        if (ship == null) return;

        float dt = Time.deltaTime;

        // 1) HELM INPUT: jen když někdo drží kormidlo
        float steerInput = 0f;

        if (ship.helmsman != null && Keyboard.current != null)
        {
            // helmsman je ten, kdo drží helm - klávesy podle hráče
            if (ship.helmsman.isPlayerOne)
            {
                steerInput = (Keyboard.current.dKey.isPressed ? 1f : 0f) - (Keyboard.current.aKey.isPressed ? 1f : 0f);
            }
            else
            {
                steerInput = (Keyboard.current.rightArrowKey.isPressed ? 1f : 0f) - (Keyboard.current.leftArrowKey.isPressed ? 1f : 0f);
            }

            // MÍSTO přímého otáčení: upravujeme stav kormidla (-3..+3)
            ship.UpdateHelmFromInput(steerInput, dt);
        }
        else
        {
            // Pokud chceš samonávrat do středu, nastav helmReturnPerSec > 0.
            // Když helmReturnPerSec = 0, kormidlo zůstane natočené navždy.
            ship.AutoCenterHelm(dt);
        }
        
        // 1.5) SAILS INPUT: jen když někdo ovládá plachty
        // 1.5) SAILS INPUT: jen když někdo ovládá plachty
        if (ship.sailOperator != null && Keyboard.current != null)
        {
            float sailInput = 0f;

            // P1: A / D
            if (ship.sailOperator.isPlayerOne)
            {
                sailInput = (Keyboard.current.dKey.isPressed ? 1f : 0f) -
                            (Keyboard.current.aKey.isPressed ? 1f : 0f);
            }
            // P2: šipky ← →
            else
            {
                sailInput = (Keyboard.current.rightArrowKey.isPressed ? 1f : 0f) -
                            (Keyboard.current.leftArrowKey.isPressed ? 1f : 0f);
            }

            ship.UpdateSailsFromInput(sailInput, dt);
        }


        // 2) TURN: loď (svět) se otáčí podle nastavení kormidla, ne podle stisknuté klávesy
        float rotationAmount = ship.helm * ship.turnPerHelmUnit * dt;

        // svět se otáčí proti lodi
        transform.RotateAround(pivotPoint, Vector3.forward, rotationAmount);

        // udrž heading synchronizovaný (užitečné pro kompas / vítr)
        ship.headingDeg -= rotationAmount;
        if (ship.headingDeg > 180f) ship.headingDeg -= 360f;
        if (ship.headingDeg < -180f) ship.headingDeg += 360f;
        
        // --- SPEED FROM WIND + SAILS ---

// směr lodi ve světě: protože loď je fixně doprava na obrazovce,
// její "heading" je ship.headingDeg (logický kurz)
        float shipRad = ship.headingDeg * Mathf.Deg2Rad;
        Vector2 shipDir = new Vector2(Mathf.Cos(shipRad), Mathf.Sin(shipRad));

// směr větru
        float windRad = ship.windDirDeg * Mathf.Deg2Rad;
        Vector2 windDir = new Vector2(Mathf.Cos(windRad), Mathf.Sin(windRad));

// úhel mezi lodí a větrem (0 = vítr do zad, 180 = do čumáku)
        float angle = Vector2.Angle(shipDir, windDir);

// penalizace za jízdu proti větru (no-go zóna)
        float pointing = 0f;
        if (angle > noGoZoneDeg)
        {
            pointing = Mathf.InverseLerp(noGoZoneDeg, 180f, angle); // 0..1
        }

// trim plachet: jednoduchý model
// ideální trim = když sailTrim odpovídá straně větru (zjednodušeně)
        float windSide = Mathf.Sign(Vector3.Cross(shipDir, windDir).z); // -1 nebo +1
        float desiredTrim = windSide; // chceme -1 když fouká zleva, +1 když zprava
        float trimError = Mathf.Abs(desiredTrim - ship.sailTrim); // 0..2
        float trimFactor = 1f - Mathf.Clamp01(trimError / 2f);    // 1..0

        float targetSpeed = maxSpeed * ship.windStrength * pointing * trimFactor;

// plynulé zrychlení / zpomalení
        ship.speed = Mathf.MoveTowards(ship.speed, targetSpeed, accel * dt);
        
        // 3) FORWARD: svět teče vždy doleva PO OBRAZOVCE (Space.World)
        float spd = useShipSpeed ? ship.speed : forwardSpeed;
        transform.Translate(Vector3.left * spd * dt, Space.World);
        
        if (Time.frameCount % 30 == 0)
            Debug.Log($"helm={ship.helm:F2}");
    }
}
