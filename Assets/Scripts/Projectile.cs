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
        // Ensure projectile is destroyed after its lifetime
        Destroy(gameObject, lifeTime);

        var rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.bodyType = RigidbodyType2D.Kinematic;
    }

    void Update()
    {
        // Move forward in local right direction
        transform.position += transform.right * speed * Time.deltaTime;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        var enemy = other.GetComponentInParent<EnemyShipAI>();
        if (enemy != null)
        {
            Destroy(enemy.gameObject);
            Destroy(gameObject);
        }
    }
}
