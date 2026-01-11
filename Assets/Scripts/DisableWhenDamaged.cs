using UnityEngine;

public enum ForcedReleaseMode
{
    None,
    Helm,
    Sails,
    CannonLeft,
    CannonRight,
    CannonFront
}

public class DisableWhenDamaged : MonoBehaviour
{
    [Header("Damage Source")]
    public DamagePoint damagePoint;

    [Header("When HEALTHY (not damaged)")]
    public Collider2D[] collidersHealthy;     // interakční triggry (helm/sails/cannon)
    public Behaviour[] behavioursHealthy;     // HelmInteractable, SailsInteractable, ...

    [Header("When DAMAGED (repair mode)")]
    public Collider2D[] collidersDamaged;     // repair triggry (defaultně vypnuté)
    public Behaviour[] behavioursDamaged;     // RepairPointInteractable (volitelné)

    [Header("Force release (kick player out)")]
    public ShipController ship;
    public ForcedReleaseMode releaseMode = ForcedReleaseMode.None;

    bool lastDamaged;

    void Start()
    {
        lastDamaged = damagePoint != null && damagePoint.isDamaged;
        ApplyState(lastDamaged);
    }

    void Update()
    {
        if (damagePoint == null) return;

        bool damaged = damagePoint.isDamaged;
        if (damaged == lastDamaged) return;

        // nově se poškodilo -> vyhoď hráče z obsluhy
        if (damaged)
            ForceReleaseIfNeeded();

        ApplyState(damaged);
        lastDamaged = damaged;
    }

    void ApplyState(bool damaged)
    {
        // zdravé = interakce ON, opravy OFF
        SetEnabled(collidersHealthy, !damaged);
        SetEnabled(behavioursHealthy, !damaged);

        // poškozené = opravy ON, interakce OFF
        SetEnabled(collidersDamaged, damaged);
        SetEnabled(behavioursDamaged, damaged);
    }

    void SetEnabled(Collider2D[] cols, bool enabled)
    {
        if (cols == null) return;
        foreach (var c in cols)
            if (c != null) c.enabled = enabled;
    }

    void SetEnabled(Behaviour[] behs, bool enabled)
    {
        if (behs == null) return;
        foreach (var b in behs)
            if (b != null) b.enabled = enabled;
    }

    void ForceReleaseIfNeeded()
    {
        if (ship == null) return;

        switch (releaseMode)
        {
            case ForcedReleaseMode.Helm:
                if (ship.helmsman != null) ship.ClearHelmsman(ship.helmsman);
                break;

            case ForcedReleaseMode.Sails:
                if (ship.sailOperator != null) ship.ClearSailOperator(ship.sailOperator);
                break;
            
            case ForcedReleaseMode.CannonLeft:
                if (ship.leftCannonOperator != null)
                    ship.ClearCannonOperator(CannonSlot.Left, ship.leftCannonOperator);
                break;

            case ForcedReleaseMode.CannonRight:
                if (ship.rightCannonOperator != null)
                    ship.ClearCannonOperator(CannonSlot.Right, ship.rightCannonOperator);
                break;

            case ForcedReleaseMode.CannonFront:
                if (ship.frontCannonOperator != null)
                    ship.ClearCannonOperator(CannonSlot.Front, ship.frontCannonOperator);
                break;
        }
    }
}
