using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public Transform shipTarget;
    public ShipDamageManager damageManager;
    public GameObject enemyPrefab;

    [Header("Spawn")]
    public float spawnRadius = 14f;
    public float spawnInterval = 4f;
    public int maxAlive = 6;

    // Parent transform for all spawned enemies
    public Transform worldContent;

    int alive;
    float timer;

    void Start()
    {
        // Auto-find ship if not assigned
        if (shipTarget == null)
        {
            var ship = FindFirstObjectByType<ShipController>();
            if (ship != null) shipTarget = ship.transform;
        }

        if (damageManager == null && shipTarget != null)
            damageManager = shipTarget.GetComponent<ShipDamageManager>();
    }

    void Update()
    {
        if (shipTarget == null || enemyPrefab == null) return;

        timer -= Time.deltaTime;
        if (timer > 0f) return;

        timer = spawnInterval;
        if (alive >= maxAlive) return;

        Spawn();
    }

    void Spawn()
    {
        float a = Random.Range(0f, 360f) * Mathf.Deg2Rad;
        Vector3 offset = new Vector3(Mathf.Cos(a), Mathf.Sin(a), 0f) * spawnRadius;
        Vector3 pos = shipTarget.position + offset;

        var go = Instantiate(enemyPrefab, pos, Quaternion.identity, worldContent);

        var ai = go.GetComponent<EnemyShipAI>();
        if (ai != null)
        {
            ai.shipTarget = shipTarget;
            ai.shipDamageManager = damageManager;
        }

        alive++;
        go.AddComponent<OnDestroyedCallback>().Init(() => alive--);
    }
}

public class OnDestroyedCallback : MonoBehaviour
{
    System.Action onDestroyed;

    public void Init(System.Action a) => onDestroyed = a;

    void OnDestroy()
    {
        onDestroyed?.Invoke();
    }
}
