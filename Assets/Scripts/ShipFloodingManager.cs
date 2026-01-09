using System.Collections.Generic;
using UnityEngine;

public class ShipFloodingManager : MonoBehaviour
{
    [Header("References")]
    public ShipDamageManager damageManager;

    [Header("Water")]
    [Range(0f, 1f)]
    public float water01 = 0f;

    [Tooltip("Kolik vody přidá okamžitě jeden nový zásah (cannon/hull/steering).")]
    public float hitAdd = 0.08f;

    [Tooltip("Extra voda za zásah do díry (HullHole).")]
    public float hullHitExtraAdd = 0.06f;

    [Tooltip("Kolik vody přibývá za sekundu za 1 neopravenou díru v trupu.")]
    public float leakPerHolePerSec = 0.05f;

    [Tooltip("Když nejsou žádné díry, jak rychle se voda sama ztrácí.")]
    public float drainPerSec = 0.03f;

    // pamatujeme si, co bylo poškozené minulý frame (detekce nového zásahu)
    readonly Dictionary<DamagePoint, bool> lastDamaged = new Dictionary<DamagePoint, bool>();

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

        // 1) Detekce nových zásahů (přechod false -> true)
        foreach (var p in damageManager.points)
        {
            if (p == null) continue;

            bool prev = lastDamaged.TryGetValue(p, out var v) ? v : false;
            bool now = p.isDamaged;

            if (!prev && now)
            {
                OnNewDamage(p);
            }

            lastDamaged[p] = now;
        }

        // 2) Průběžné zatékání podle počtu děr v trupu
        int openHoles = CountDamagedHoles();
        if (openHoles > 0)
        {
            water01 += leakPerHolePerSec * openHoles * Time.deltaTime;
        }
        else
        {
            // 3) Pokud jsou všechny díry opravené, voda pomalu mizí
            water01 -= drainPerSec * Time.deltaTime;
        }

        water01 = Mathf.Clamp01(water01);
    }

    void OnNewDamage(DamagePoint p)
    {
        // Ignorujeme plachty/stěžně (Mast)
        if (p.type == DamageType.Mast) return;

        float add = hitAdd;

        // díra do trupu = větší problém
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
