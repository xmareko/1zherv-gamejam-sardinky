using UnityEngine;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class Projectile : MonoBehaviour
{
    [Header("Flight")]
    public float speed = 10f;
    public float lifeTime = 2.5f;

    void Start()
    {
        // jistota, že se zničí
        Destroy(gameObject, lifeTime);

        // nastavení rigidbody (bez gravitace)
        var rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.bodyType = RigidbodyType2D.Kinematic;
    }

    void Update()
    {
        // letí směrem, kam kouká dělo
        transform.position += transform.right * speed * Time.deltaTime;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // trefil enemy?
        var enemy = other.GetComponentInParent<EnemyShipAI>();
        if (enemy != null)
        {
            Destroy(enemy.gameObject); // znič enemy loď
            Destroy(gameObject);       // znič střelu
            return;
        }

        // volitelné: trefil něco jiného (např. ostrov)
        // Destroy(gameObject);
    }
}