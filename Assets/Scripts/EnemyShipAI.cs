using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class EnemyShipAI : MonoBehaviour
{
    [Header("Target")]
    public Transform shipTarget;                 // ShipRoot (Transform)
    public ShipDamageManager shipDamageManager;  // ShipDamageManager na lodi

    [Header("Movement")]
    public float moveSpeed = 2.5f;

    [Header("Behavior")]
    [Tooltip("Když true, enemy je ovlivněný 'world drift' (můžeš mu ujet). Když false, world drift se neaplikuje (neujedeš mu).")]
    public bool canBeOutrun = true;

    [Header("World drift (used only when canBeOutrun=true)")]
    public WorldMover worldMover;        // můžeš vyplnit v Inspectoru, jinak se najde v Start()
    public float extraWorldDrag = 0f;    // volitelné: když chceš aby zaostával víc

    [Header("Hit")]
    public bool useTriggerHit = true;    // doporučuji true
    bool hasHit;

    [Header("Rotation")]
    public bool rotateTowardsTarget = true;
    public float spriteForwardOffsetDeg = 270f;

    void Start()
    {
        // když není ručně vyplněno, zkus si to najít
        if (shipTarget != null && shipDamageManager == null)
            shipDamageManager = shipTarget.GetComponent<ShipDamageManager>();

        // worldMover potřebujeme jen když canBeOutrun=true
        if (canBeOutrun && worldMover == null)
            worldMover = FindFirstObjectByType<WorldMover>();

        // trigger je nejjednodušší
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

        // 0) Volitelně: svět se posouvá doleva podle rychlosti lodi
        // Když canBeOutrun=false, tohle se úplně přeskočí -> enemy se bude pořád přibližovat jako dřív.
        if (canBeOutrun && worldMover != null && worldMover.ship != null)
        {
            float spd = worldMover.useDynamicSpeed ? worldMover.ship.speed : worldMover.forwardSpeed;
            transform.position += Vector3.left * (spd + extraWorldDrag) * dt;
        }

        // 1) Vlastní motor směrem k lodi
        Vector3 toShip = shipTarget.position - transform.position;
        float dist = toShip.magnitude;
        if (dist < 0.0001f) return;

        Vector3 dir = toShip / dist;
        transform.position += dir * moveSpeed * dt;

        // 2) Rotace špičkou na loď
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

        if (shipTarget != null && (other.transform == shipTarget || other.transform.IsChildOf(shipTarget)))
            HitShip();
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (useTriggerHit) return;
        if (hasHit) return;

        if (shipTarget != null && (col.transform == shipTarget || col.transform.IsChildOf(shipTarget)))
            HitShip();
    }

    void HitShip()
    {
        if (hasHit) return;
        hasHit = true;

        if (shipDamageManager != null)
            shipDamageManager.DamageRandomPoint();
        else
            Debug.LogWarning("EnemyShipAI: shipDamageManager is NULL!");

        Destroy(gameObject);
    }
}
