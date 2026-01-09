using UnityEngine;
using UnityEngine.UI;

public class WaterBarUI : MonoBehaviour
{
    public ShipFloodingManager flooding;
    public Image fillImage; // WaterBarFill

    void Update()
    {
        if (flooding == null || fillImage == null) return;
        fillImage.fillAmount = flooding.water01;
    }
}