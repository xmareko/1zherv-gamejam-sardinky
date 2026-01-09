using System.Collections.Generic;
using UnityEngine;

public class ShipDamageManager : MonoBehaviour
{
    public List<DamagePoint> points = new List<DamagePoint>();

    [Header("Debug / test")]
    public bool autoDamageForTest = false;
    public float damageEvery = 6f;

    float t;

    void Update()
    {
        if (!autoDamageForTest) return;

        t += Time.deltaTime;
        if (t >= damageEvery)
        {
            t = 0f;
            DamageRandomPoint();
        }
    }

    public void DamageRandomPoint()
    {
        if (points.Count == 0) return;

        // vyber náhodný, co ještě není poškozený
        var candidates = points.FindAll(p => p != null && !p.isDamaged);
        if (candidates.Count == 0) return;

        var pick = candidates[Random.Range(0, candidates.Count)];
        pick.Damage();
        Debug.Log($"DAMAGED: {pick.name} ({pick.type})");
    }

    // Penalizace pro loď podle poškození
    public float SpeedMultiplier()
    {
        float mult = 1f;

        foreach (var p in points)
        {
            if (p == null || !p.isDamaged) continue;

            if (p.type == DamageType.Mast) mult *= 0.85f;
            if (p.type == DamageType.HullHole) mult *= 0.90f;
        }
        return Mathf.Clamp(mult, 0.2f, 1f);
    }

    public float TurnMultiplier()
    {
        float mult = 1f;

        foreach (var p in points)
        {
            if (p == null || !p.isDamaged) continue;

            if (p.type == DamageType.Steering) mult *= 0.6f;
        }
        return Mathf.Clamp(mult, 0.2f, 1f);
    }

    public bool IsCannonBroken(string cannonName)
    {
        foreach (var p in points)
        {
            if (p == null) continue;
            if (p.type == DamageType.Cannon && p.isDamaged && p.name.Contains(cannonName))
                return true;
        }
        return false;
    }
}