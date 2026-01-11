using UnityEngine;

public class ShipController : MonoBehaviour
{
    [Header("Wind (set by WindSystem)")]
    [Tooltip("Směr větru ve světě ve stupních. 0 = doprava, 90 = nahoru.")]
    public float windDirDeg = 0f;

    [Tooltip("Síla větru 0..1")]
    [Range(0f, 1f)]
    public float windStrength = 1f;

    [Header("Sails")]
    [Tooltip("Trim plachet -1..+1 (zjednodušené).")]
    public float sailTrim = 0f;
    public float sailTrimMax = 1f;
    public float sailChangePerSec = 1.5f;
    public PlayerInteractor sailOperator; // kdo ovládá plachty

    [Header("Helm")]
    [Tooltip("Aktuální nastavení kormidla. Zůstává i když hráč odejde (pokud se necentruje).")]
    public float helm = 0f;
    public float helmMax = 0.5f;
    public float helmChangePerSec = 1.5f;
    public float helmReturnPerSec = 0.2f; // 0 = nikdy se nevrací
    public float turnPerHelmUnit = 25f;   // deg/sec při helm=1

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
    public float headingDeg = 0f; // logický kurz (pro kompas/vítr)
    public float speed = 2f;      // aktuální rychlost (počítá WorldMover)

    [Header("Runtime")]
    public PlayerInteractor helmsman; // kdo drží kormidlo

    // ---------- Helm ownership ----------
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

    // ---------- Sails ownership ----------
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

    // ---------- Cannons ownership ----------
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

    // Volitelné: když chceš hráče "odhlásit" ze všech stanovišť najednou
    public void ClearAllStations(PlayerInteractor interactor)
    {
        if (helmsman == interactor) ClearHelmsman(interactor);
        if (sailOperator == interactor) ClearSailOperator(interactor);

        if (leftCannonOperator == interactor) ClearCannonOperator(CannonSlot.Left, interactor);
        if (rightCannonOperator == interactor) ClearCannonOperator(CannonSlot.Right, interactor);
        if (frontCannonOperator == interactor) ClearCannonOperator(CannonSlot.Front, interactor);
    }

    // ---------- Helm & sails state updates ----------
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

    // ---------- helpers ----------
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
