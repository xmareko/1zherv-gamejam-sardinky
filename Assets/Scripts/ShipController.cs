using UnityEngine;

public class ShipController : MonoBehaviour
{
    [Header("Wind (set by WindSystem)")]
    // World-space wind direction in degrees (0 = right, 90 = up)
    public float windDirDeg = 0f;

    // Normalized wind strength
    [Range(0f, 1f)]
    public float windStrength = 1f;

    [Header("Sails")]
    // Simplified sail trim value (-1..+1)
    public float sailTrim = 0f;
    public float sailTrimMax = 1f;
    public float sailChangePerSec = 1.5f;
    public PlayerInteractor sailOperator;

    [Header("Helm")]
    // Helm input persists even after player leaves
    public float helm = 0f;
    public float helmMax = 0.5f;
    public float helmChangePerSec = 1.5f;
    public float helmReturnPerSec = 0.2f;
    public float turnPerHelmUnit = 25f;

    [Header("Cannons")]
    public PlayerInteractor leftCannonOperator;
    public PlayerInteractor rightCannonOperator;
    public PlayerInteractor frontCannonOperator;

    public CannonController leftCannon;
    public CannonController rightCannon;
    public CannonController frontCannon;

    public CannonShooter leftCannonShooter;
    public CannonShooter rightCannonShooter;
    public CannonShooter frontCannonShooter;

    [Header("Ship State")]
    // Logical heading used for navigation systems
    public float headingDeg = 0f;
    public float speed = 2f;

    [Header("Runtime")]
    public PlayerInteractor helmsman;

    // --- Helm ownership ---
    public void SetHelmsman(PlayerInteractor interactor)
    {
        helmsman = interactor;
        DisablePlayerMove(interactor);
    }

    public void ClearHelmsman(PlayerInteractor interactor)
    {
        if (helmsman != interactor) return;
        EnablePlayerMove(interactor);
        helmsman = null;
    }

    // --- Sails ownership ---
    public void SetSailOperator(PlayerInteractor interactor)
    {
        sailOperator = interactor;
        DisablePlayerMove(interactor);
    }

    public void ClearSailOperator(PlayerInteractor interactor)
    {
        if (sailOperator != interactor) return;
        EnablePlayerMove(interactor);
        sailOperator = null;
    }

    // --- Cannons ownership ---
    public PlayerInteractor GetCannonOperator(CannonSlot slot)
    {
        switch (slot)
        {
            case CannonSlot.Left: return leftCannonOperator;
            case CannonSlot.Right: return rightCannonOperator;
            case CannonSlot.Front: return frontCannonOperator;
        }
        return null;
    }

    public void SetCannonOperator(CannonSlot slot, PlayerInteractor interactor, CannonController cannon)
    {
        switch (slot)
        {
            case CannonSlot.Left:
                leftCannonOperator = interactor;
                if (cannon != null) leftCannon = cannon;
                break;

            case CannonSlot.Right:
                rightCannonOperator = interactor;
                if (cannon != null) rightCannon = cannon;
                break;

            case CannonSlot.Front:
                frontCannonOperator = interactor;
                if (cannon != null) frontCannon = cannon;
                break;
        }

        DisablePlayerMove(interactor);
    }

    public void ClearCannonOperator(CannonSlot slot, PlayerInteractor interactor)
    {
        switch (slot)
        {
            case CannonSlot.Left:
                if (leftCannonOperator != interactor) return;
                EnablePlayerMove(interactor);
                leftCannonOperator = null;
                return;

            case CannonSlot.Right:
                if (rightCannonOperator != interactor) return;
                EnablePlayerMove(interactor);
                rightCannonOperator = null;
                return;

            case CannonSlot.Front:
                if (frontCannonOperator != interactor) return;
                EnablePlayerMove(interactor);
                frontCannonOperator = null;
                return;
        }
    }

    // Clears player from all stations at once
    public void ClearAllStations(PlayerInteractor interactor)
    {
        if (helmsman == interactor) ClearHelmsman(interactor);
        if (sailOperator == interactor) ClearSailOperator(interactor);

        if (leftCannonOperator == interactor) ClearCannonOperator(CannonSlot.Left, interactor);
        if (rightCannonOperator == interactor) ClearCannonOperator(CannonSlot.Right, interactor);
        if (frontCannonOperator == interactor) ClearCannonOperator(CannonSlot.Front, interactor);
    }

    // --- Input-driven state updates ---
    public void UpdateHelmFromInput(float steerInput, float dt)
    {
        helm += steerInput * helmChangePerSec * dt;
        helm = Mathf.Clamp(helm, -helmMax, helmMax);
    }

    public void AutoCenterHelm(float dt)
    {
        if (helmReturnPerSec <= 0f) return;
        helm = Mathf.MoveTowards(helm, 0f, helmReturnPerSec * dt);
    }

    public void UpdateSailsFromInput(float input, float dt)
    {
        sailTrim += input * sailChangePerSec * dt;
        sailTrim = Mathf.Clamp(sailTrim, -sailTrimMax, sailTrimMax);
    }

    // --- Player movement locking ---
    void DisablePlayerMove(PlayerInteractor interactor)
    {
        var pc = interactor.GetComponent<PlayerController>();
        if (pc != null)
        {
            pc.enabled = false;
            var rb = interactor.GetComponent<Rigidbody2D>();
            if (rb != null) rb.linearVelocity = Vector2.zero;
        }
    }

    void EnablePlayerMove(PlayerInteractor interactor)
    {
        var pc = interactor.GetComponent<PlayerController>();
        if (pc != null) pc.enabled = true;
    }
}
