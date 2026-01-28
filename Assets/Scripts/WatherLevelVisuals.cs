using UnityEngine;

public class WaterLevelVisuals : MonoBehaviour
{
    [Header("References")]
    public ShipFloodingManager floodingManager;
    public SpriteRenderer waterSpriteRenderer;

    [Header("Appearance Settings")]
    [Tooltip("The opacity of the sprite when the ship is fully flooded (0 to 1).")]
    [Range(0f, 1f)] public float maxOpacity = 0.9f;

    void Update()
    {
        if (floodingManager == null || waterSpriteRenderer == null) return;

        // 1. Calculate the flood percentage (0.0 to 1.0)
        // We use the data from ShipFloodingManager
        float floodPercent = Mathf.Clamp01(floodingManager.currentWaterLevel / floodingManager.maxWaterLevel);

        // 2. Calculate the new Alpha value
        float newAlpha = floodPercent * maxOpacity;

        // 3. Apply the color with the new alpha to the sprite
        Color c = waterSpriteRenderer.color;
        c.a = newAlpha;
        waterSpriteRenderer.color = c;
    }
}