using UnityEngine;
using UnityEngine.InputSystem;

public class WorldMover : MonoBehaviour
{
    [Header("References")]
    public ShipController ship;   // ShipRoot s ShipControllerem

    [Header("Forward Movement")]
    public bool useDynamicSpeed = true;
    public float forwardSpeed = 5f;  // použije se jen když useShipSpeed=false

    [Header("Sailing Model")]
    public float maxSpeed = 8f;
    public float accel = 1.5f;       // jak rychle loď zrychluje/brzdí
    public float noGoZoneDeg = 90f;  // úhel proti větru, kde skoro nejede

    // Pivot: střed lodi (loď je fixně na (0,0,0))
    private readonly Vector3 pivotPoint = Vector3.zero;

    void Update()
    {
        if (ship == null) return;
        if (Keyboard.current == null)
        {
            MoveForward(Time.deltaTime);
            return;
        }

        float dt = Time.deltaTime;

        // 1) HELM INPUT (jen když někdo drží kormidlo)
        if (ship.helmsman != null)
        {
            float steerInput = ReadLeftRight(ship.helmsman.isPlayerOne);
            ship.UpdateHelmFromInput(steerInput, dt);
        }
        else
        {
            ship.AutoCenterHelm(dt);
        }

        // 2) SAILS INPUT (jen když někdo ovládá plachty)
        // Pozn.: je OK, že používají stejné klávesy, protože PlayerController je při obsluze vypnutý.
        if (ship.sailOperator != null)
        {
            float sailInput = ReadLeftRight(ship.sailOperator.isPlayerOne);
            ship.UpdateSailsFromInput(-sailInput, dt);
        }
        
        // 2.5) CANNONS INPUT (jen když někdo ovládá dělo)
        if (ship.leftCannonOperator != null && ship.leftCannon != null)
        {
            float input = ReadLeftRight(ship.leftCannonOperator.isPlayerOne);
            ship.leftCannon.Rotate(-input, dt);
        }

        if (ship.rightCannonOperator != null && ship.rightCannon != null)
        {
            float input = ReadLeftRight(ship.rightCannonOperator.isPlayerOne);
            ship.rightCannon.Rotate(-input, dt);
        }

        if (ship.frontCannonOperator != null && ship.frontCannon != null)
        {
            float input = ReadLeftRight(ship.frontCannonOperator.isPlayerOne);
            ship.frontCannon.Rotate(-input, dt);
        }
        
        // 2.6) CANNONS FIRE
        if (ship.leftCannonOperator != null && ship.leftCannonShooter != null)
        {
            if (WasFirePressed(ship.leftCannonOperator.isPlayerOne))
                ship.leftCannonShooter.Shoot();
        }

        if (ship.rightCannonOperator != null && ship.rightCannonShooter != null)
        {
            if (WasFirePressed(ship.rightCannonOperator.isPlayerOne))
                ship.rightCannonShooter.Shoot();
        }

        if (ship.frontCannonOperator != null && ship.frontCannonShooter != null)
        {
            if (WasFirePressed(ship.frontCannonOperator.isPlayerOne))
                ship.frontCannonShooter.Shoot();
        }



        // 3) TURN: otáčení světa podle kormidla (stavového)
        float rotationAmount = ship.helm * ship.turnPerHelmUnit * dt;
        transform.RotateAround(pivotPoint, Vector3.forward, rotationAmount);

        // drž logický kurz lodi
        ship.headingDeg -= rotationAmount;
        ship.headingDeg = Wrap180(ship.headingDeg);

        // 4) SPEED from wind + sails
        float targetSpeed = ComputeTargetSpeed();
        ship.speed = Mathf.MoveTowards(ship.speed, targetSpeed, accel * dt);

        // 5) FORWARD: svět vždy teče doleva po obrazovce
        MoveForward(dt);

        // Debug (můžeš smazat)
        // if (Time.frameCount % 60 == 0)
        //     Debug.Log($"windDir={ship.windDirDeg:F0} windStr={ship.windStrength:F2} trim={ship.sailTrim:F2} speed={ship.speed:F2} helm={ship.helm:F2}");
    }

    float ReadLeftRight(bool isPlayerOne)
    {
        // P1 = A/D, P2 = šipky
        if (isPlayerOne)
        {
            return (Keyboard.current.dKey.isPressed ? 1f : 0f) -
                   (Keyboard.current.aKey.isPressed ? 1f : 0f);
        }
        else
        {
            return (Keyboard.current.rightArrowKey.isPressed ? 1f : 0f) -
                   (Keyboard.current.leftArrowKey.isPressed ? 1f : 0f);
        }
    }

    void MoveForward(float dt)
    {
        float spd = useDynamicSpeed ? ship.speed : forwardSpeed;
        transform.Translate(Vector3.left * spd * dt, Space.World);
    }

    float ComputeTargetSpeed()
    {
        // směr lodi ve světě
        float shipRad = ship.headingDeg * Mathf.Deg2Rad;
        Vector2 shipDir = new Vector2(Mathf.Cos(shipRad), Mathf.Sin(shipRad));

        // Pokud windDirDeg znamená ODKUD vítr fouká (FROM),
        // tak směr KAM fouká (TO) je +180°
        float windToDeg = ship.windDirDeg + 180f;
        float windRad = windToDeg * Mathf.Deg2Rad;
        Vector2 windDir = new Vector2(Mathf.Cos(windRad), Mathf.Sin(windRad));

        // úhel mezi lodí a větrem (0..180)
        // 0°  = vítr fouká stejným směrem jako loď (tailwind - dobré)
        // 180° = vítr fouká proti lodi (headwind - špatné)
        float angle = Vector2.Angle(shipDir, windDir);

        // OPRAVA: pointing má být 1 při tailwind (0°) a 0 při headwind (180°)
        // noGoZoneDeg tady ber jako "od kdy to začíná být fakt špatné" směrem k headwind.
        float pointing = 1f - Mathf.InverseLerp(noGoZoneDeg, 180f, angle);
        pointing = Mathf.Clamp01(pointing);

        // trim plachet: chceme trim podle strany větru
        float crossZ = Vector3.Cross(shipDir, windDir).z;

        // Pokud vítr jde přesně před/za (cross ~ 0), ideální trim je 0 (plachty "na střed")
        float desiredTrim = 0f;
        if (Mathf.Abs(crossZ) >= 0.001f)
            desiredTrim = Mathf.Sign(crossZ); // -1 nebo +1

        float trimError = Mathf.Abs(desiredTrim - ship.sailTrim); // 0..2
        float trimFactor = 1f - Mathf.Clamp01(trimError / 2f);    // 1..0

        float baseSpeed = 0.2f; // minimální drift
        return baseSpeed + maxSpeed * ship.windStrength * pointing * trimFactor;
    }


    float Wrap180(float deg)
    {
        while (deg > 180f) deg -= 360f;
        while (deg < -180f) deg += 360f;
        return deg;
    }
    
    bool WasFirePressed(bool isPlayerOne)
    {
        if (Keyboard.current == null) return false;

        // P1: Space, P2: Enter
        if (isPlayerOne)
            return Keyboard.current.wKey.wasPressedThisFrame;
        else
            return Keyboard.current.upArrowKey.wasPressedThisFrame;
    }

}
