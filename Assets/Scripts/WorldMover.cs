using UnityEngine;
using UnityEngine.InputSystem;

public class WorldMover : MonoBehaviour
{
    [Header("References")]
    public ShipController ship;

    [Header("Forward Movement")]
    public bool useDynamicSpeed = true;
    public float forwardSpeed = 5f;

    [Header("Sailing Model")]
    public float maxSpeed = 8f;
    public float accel = 1.5f;
    public float noGoZoneDeg = 90f;

    // Ship is fixed at origin; the world rotates around this pivot
    readonly Vector3 pivotPoint = Vector3.zero;

    void Update()
    {
        if (ship == null) return;

        if (Keyboard.current == null)
        {
            MoveForward(Time.deltaTime);
            return;
        }

        float dt = Time.deltaTime;

        // Helm input is only read while a player is assigned to the helm
        if (ship.helmsman != null)
        {
            float steerInput = ReadLeftRight(ship.helmsman.isPlayerOne);
            ship.UpdateHelmFromInput(steerInput, dt);
        }
        else
        {
            ship.AutoCenterHelm(dt);
        }

        // Sails input is only read while a player is assigned to sails
        if (ship.sailOperator != null)
        {
            float sailInput = ReadLeftRight(ship.sailOperator.isPlayerOne);
            ship.UpdateSailsFromInput(-sailInput, dt);
        }

        // Cannon rotation uses the same left/right input as helm/sails
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

        // Cannon fire is gated by operator ownership
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

        // Rotate the world based on helm state and keep ship heading in sync
        float rotationAmount = ship.helm * ship.turnPerHelmUnit * dt;
        transform.RotateAround(pivotPoint, Vector3.forward, rotationAmount);

        ship.headingDeg -= rotationAmount;
        ship.headingDeg = Wrap180(ship.headingDeg);

        // Speed is driven by wind direction/strength and current sail trim
        float targetSpeed = ComputeTargetSpeed();
        ship.speed = Mathf.MoveTowards(ship.speed, targetSpeed, accel * dt);

        // World scroll gives the illusion of forward movement
        MoveForward(dt);
    }

    float ReadLeftRight(bool isPlayerOne)
    {
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
        Vector2 shipDir = new Vector2(
            Mathf.Cos(ship.headingDeg * Mathf.Deg2Rad),
            Mathf.Sin(ship.headingDeg * Mathf.Deg2Rad)
        );

        // Wind "to" direction is opposite of "from" direction
        float windToDeg = ship.windDirDeg + 180f;
        Vector2 windDir = new Vector2(
            Mathf.Cos(windToDeg * Mathf.Deg2Rad),
            Mathf.Sin(windToDeg * Mathf.Deg2Rad)
        );

        float angle = Vector2.Angle(shipDir, windDir);

        // Tailwind (0°) => 1, headwind (180°) => 0
        float pointing = 1f - Mathf.InverseLerp(noGoZoneDeg, 180f, angle);
        pointing = Mathf.Clamp01(pointing);

        float crossZ = Vector3.Cross(shipDir, windDir).z;

        float desiredTrim = 0f;
        if (Mathf.Abs(crossZ) >= 0.001f)
            desiredTrim = Mathf.Sign(crossZ);

        float trimError = Mathf.Abs(desiredTrim - ship.sailTrim);
        float trimFactor = 1f - Mathf.Clamp01(trimError / 2f);

        float baseSpeed = 0.2f;
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

        // P1: W, P2: UpArrow (matches your current bindings)
        if (isPlayerOne)
            return Keyboard.current.wKey.wasPressedThisFrame;
        else
            return Keyboard.current.upArrowKey.wasPressedThisFrame;
    }
}
