using UnityEngine;

public class WindVisual : MonoBehaviour
{
    public ShipController ship;
    public Renderer rend;

    [Header("Settings")]
    public float baseSpeed = 0.1f;       // Minimum movement speed (even if wind is 0)
    public float windMultiplier = 2.0f;  // How fast it moves at max wind strength

    [Tooltip("Texture tiling aspect ratio fix if needed")]
    public Vector2 textureScale = new Vector2(1, 1);

    void Start()
    {
        if (rend == null) rend = GetComponent<Renderer>();

        // Ensure material has texture repeating enabled
        if (rend != null)
        {
            rend.material.mainTextureScale = textureScale;
        }
    }

    void LateUpdate()
    {
        if (ship == null || rend == null) return;

        // 1. ROTATION (Relative to Ship)
        // We calculate the angle of the wind relative to where the ship is facing.
        // If Wind is 0° (Right) and Ship is 0° (Right), the visual stays at 0°.
        float relativeAngle = ship.windDirDeg - ship.headingDeg;

        transform.rotation = Quaternion.Euler(0, 0, relativeAngle);

        // 2. SPEED CALCULATION
        float currentSpeed = baseSpeed + (ship.windStrength * windMultiplier);

        // 3. SCROLLING
        // We want the wind to blow "forward" in its own local space.
        // In Unity UVs, adding to X moves the texture LEFT. Subtracting moves RIGHT.
        // Since 0 degrees is "Right" in Unity World space, we usually want the texture to flow towards positive X.
        // Therefore, we SUBTRACT from the offset.
        float offsetChange = currentSpeed * Time.deltaTime;

        // We modify the X offset of the texture in Local space
        Vector2 currentOffset = rend.material.mainTextureOffset;
        currentOffset.x -= offsetChange;

        // Loop value to keep numbers small
        currentOffset.x %= 1;

        rend.material.mainTextureOffset = currentOffset;
    }
}