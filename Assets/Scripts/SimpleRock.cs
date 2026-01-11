using UnityEngine;

public class SimpleRock : MonoBehaviour
{
    private bool hasHit = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasHit) return;

        // 1. Find the ShipDamageManager on the object we hit (or its parent)
        ShipDamageManager hitShipManager = other.GetComponentInParent<ShipDamageManager>();

        // 2. If we found it, deal damage
        if (hitShipManager != null)
        {
            hasHit = true;

            // FIX: Use the 'hitShipManager' we just found, not the empty public variable
            hitShipManager.DamageRandomPoint();

            Debug.Log("Ship hit a rock! Damage dealt.");

            // Optional: Destroy rock after hit so it doesn't hit again
            // Destroy(gameObject);
        }
    }
}