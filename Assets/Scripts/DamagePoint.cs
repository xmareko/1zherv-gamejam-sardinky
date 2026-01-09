using UnityEngine;

public enum DamageType
{
    Mast,
    Cannon,
    HullHole,
    Steering
}

public class DamagePoint : MonoBehaviour
{
    public DamageType type;
    public bool isDamaged;

    [Header("Optional visuals")]
    public GameObject damagedVisual;

    void Start()
    {
        UpdateVisual();
    }

    public void Damage()
    {
        isDamaged = true;
        UpdateVisual();
    }

    public void Repair()
    {
        isDamaged = false;
        UpdateVisual();
    }

    void UpdateVisual()
    {
        if (damagedVisual != null)
            damagedVisual.SetActive(isDamaged);
    }
}