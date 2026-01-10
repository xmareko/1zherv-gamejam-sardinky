using UnityEngine;

public class WaterScroller : MonoBehaviour
{
    public ShipController ship;
    public Renderer waterRenderer;
    public float scrollSpeedMultiplier = 0.1f;

    void Update()
    {
        if (ship == null || waterRenderer == null) return;

        // Move texture based on ship speed
        float offsetChange = ship.speed * scrollSpeedMultiplier * Time.deltaTime;

        Vector2 currentOffset = waterRenderer.material.mainTextureOffset;
        currentOffset.x += offsetChange;

        waterRenderer.material.mainTextureOffset = currentOffset;
    }
}