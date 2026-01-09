using UnityEngine;

public enum ForcedReleaseMode
{
    None,
    Helm,
    Sails
}

public class DisableWhenDamaged : MonoBehaviour
{
    [Header("Damage Source")]
    public DamagePoint damagePoint;

    [Header("What to disable")]
    public Collider2D[] collidersToDisable;
    public Behaviour[] behavioursToDisable;

    [Header("Force release (kick player out)")]
    public ShipController ship;
    public ForcedReleaseMode releaseMode = ForcedReleaseMode.None;

    bool wasDamaged;

    void Update()
    {
        if (damagePoint == null) return;

        bool isDamaged = damagePoint.isDamaged;

        // když se právě nově poškodilo -> vyhoď hráče z obsluhy
        if (!wasDamaged && isDamaged)
        {
            ForceReleaseIfNeeded();
        }

        // enable/disable colliderů a skriptů
        if (collidersToDisable != null)
            foreach (var c in collidersToDisable)
                if (c != null) c.enabled = !isDamaged;

        if (behavioursToDisable != null)
            foreach (var b in behavioursToDisable)
                if (b != null) b.enabled = !isDamaged;

        wasDamaged = isDamaged;
    }

    void ForceReleaseIfNeeded()
    {
        if (ship == null) return;

        switch (releaseMode)
        {
            case ForcedReleaseMode.Helm:
                if (ship.helmsman != null)
                    ship.ClearHelmsman(ship.helmsman);
                break;

            case ForcedReleaseMode.Sails:
                if (ship.sailOperator != null)
                    ship.ClearSailOperator(ship.sailOperator);
                break;
        }
    }
}