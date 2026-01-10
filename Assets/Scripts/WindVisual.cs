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

        // 1. Rotation matches Wind Direction relative to Ship
        float relativeAngle = ship.windDirDeg - ship.headingDeg;
        transform.rotation = Quaternion.Euler(0, 0, relativeAngle);

        // 2. Speed based on Wind Strength
        float currentSpeed = baseSpeed + (ship.windStrength * windMultiplier);

        // 3. Scroll Texture (REVERSED DIRECTION)
        Vector2 offset = rend.material.mainTextureOffset;

        // CHANGED TO PLUS (+) TO REVERSE FLOW
        offset.x += currentSpeed * Time.deltaTime;

        offset.x %= 1;
        rend.material.mainTextureOffset = offset;
    }
}