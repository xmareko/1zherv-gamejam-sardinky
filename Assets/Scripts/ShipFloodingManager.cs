using System.Collections.Generic;
using UnityEngine;

public class ShipFloodingManager : MonoBehaviour
{
    [Header("References")]
    public ShipDamageManager damageManager;

    [Header("Water")]
    [Range(0f, 1f)]
    public float water01 = 0f;

    [Tooltip("How much water is added instantly by a new hit (cannon/hull/steering).")]
    public float hitAdd = 0.04f;

    [Tooltip("Extra water added by a hull hole hit.")]
    public float hullHitExtraAdd = 0.05f;

    [Tooltip("Water gained per second per unrepaired hull hole.")]
    public float leakPerHolePerSec = 0.03f;

    [Tooltip("If there are no holes, how fast water drains out.")]
    public float drainPerSec = 0.1f;

    // Tracks damaged state changes to detect new hits (false -> true)
    readonly Dictionary<DamagePoint, bool> lastDamaged = new Dictionary<DamagePoint, bool>();

    bool isSunk = false;

    void Awake()
    {
        if (damageManager == null)
            damageManager = GetComponent<ShipDamageManager>();
    }

    void Start()
    {
        CacheInitialStates();
    }

    void CacheInitialStates()
    {
        lastDamaged.Clear();
        if (damageManager == null) return;

        foreach (var p in damageManager.points)
        {
            if (p == null) continue;
            lastDamaged[p] = p.isDamaged;
        }
    }

    void Update()
    {
        if (damageManager == null) return;

        foreach (var p in damageManager.points)
        {
            if (p == null) continue;

            bool prev = lastDamaged.TryGetValue(p, out var v) ? v : false;
            bool now = p.isDamaged;

            if (!prev && now)
                OnNewDamage(p);

            lastDamaged[p] = now;
        }

        int openHoles = CountDamagedHoles();
        if (openHoles > 0)
            water01 += leakPerHolePerSec * openHoles * Time.deltaTime;
        else
            water01 -= drainPerSec * Time.deltaTime;

        water01 = Mathf.Clamp01(water01);

        if (!isSunk && water01 >= 1f)
            TriggerSinking();
    }

    void TriggerSinking()
    {
        isSunk = true;

        // sound
        SimpleAudio.Instance.PlayGameOver();

        if (GameManager.Instance != null)
            GameManager.Instance.GameOver("The ship has sunk into the depths!");
    }

    void OnNewDamage(DamagePoint p)
    {
        // Ignore mast damage (no flooding impact)
        if (p.type == DamageType.Mast) return;

        float add = hitAdd;

        if (p.type == DamageType.HullHole)
            add += hullHitExtraAdd;

        water01 = Mathf.Clamp01(water01 + add);
    }

    int CountDamagedHoles()
    {
        int count = 0;

        foreach (var p in damageManager.points)
        {
            if (p == null) continue;
            if (p.type == DamageType.HullHole && p.isDamaged) count++;
        }

        return count;
    }
}
