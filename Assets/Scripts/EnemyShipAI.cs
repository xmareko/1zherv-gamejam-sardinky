using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class EnemyShipAI : MonoBehaviour
{
    [Header("Target")]
    public Transform shipTarget;
    public ShipDamageManager shipDamageManager;

    [Header("Movement")]
    public float moveSpeed = 2.5f;

    [Header("Behavior")]
    // If false, enemy ignores world drift and will always catch up
    public bool canBeOutrun = true;

    [Header("World drift (used only when canBeOutrun=true)")]
    public WorldMover worldMover;
    public float extraWorldDrag = 0f;

    [Header("Hit")]
    // Trigger is preferred to avoid physics pushing
    public bool useTriggerHit = true;
    bool hasHit;

    [Header("Rotation")]
    // Rotates sprite nose towards the ship
    public bool rotateTowardsTarget = true;
    public float spriteForwardOffsetDeg = 270f;

    void Start()
    {
        // Auto-link damage manager from target ship
        if (shipTarget != null && shipDamageManager == null)
            shipDamageManager = shipTarget.GetComponent<ShipDamageManager>();

        // WorldMover is only needed when enemy can be outrun
        if (canBeOutrun && worldMover == null)
            worldMover = FindFirstObjectByType<WorldMover>();

        // Use trigger to detect hit without physical collision response
        if (useTriggerHit)
        {
            var col = GetComponent<Collider2D>();
            if (col != null) col.isTrigger = true;
        }
    }

    void Update()
    {
        if (hasHit) return;
        if (shipTarget == null) return;

        float dt = Time.deltaTime;

        // Apply world drift so enemy lags behind with the moving world
        if (canBeOutrun && worldMover != null && worldMover.ship != null)
        {
            float spd = worldMover.useDynamicSpeed ? worldMover.ship.speed : worldMover.forwardSpeed;
            transform.position += Vector3.left * (spd + extraWorldDrag) * dt;
        }

        // Move directly towards the ship
        Vector3 toShip = shipTarget.position - transform.position;
        float dist = toShip.magnitude;
        if (dist < 0.0001f) return;

        Vector3 dir = toShip / dist;
        transform.position += dir * moveSpeed * dt;

        // Face the ship visually
        if (rotateTowardsTarget)
        {
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, angle + spriteForwardOffsetDeg);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!useTriggerHit) return;
        if (hasHit) return;

        if (shipTarget != null &&
            (other.transform == shipTarget || other.transform.IsChildOf(shipTarget)))
            HitShip();
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (useTriggerHit) return;
        if (hasHit) return;

        if (shipTarget != null &&
            (col.transform == shipTarget || col.transform.IsChildOf(shipTarget)))
            HitShip();
    }

    void HitShip()
    {
        // Ensure damage is applied only once
        if (hasHit) return;
        hasHit = true;

        if (shipDamageManager != null)
            shipDamageManager.DamageRandomPoint();
        else
            Debug.LogWarning("EnemyShipAI: shipDamageManager is NULL!");

        Destroy(gameObject);
    }
}
