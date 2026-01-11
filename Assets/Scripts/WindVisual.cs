using UnityEngine;

public class WindVisual : MonoBehaviour
{
    public ShipController ship;
    public Renderer rend;

    [Header("Settings")]
    public float baseSpeed = 0.1f;
    public float windMultiplier = 2.0f;

    [Tooltip("Texture tiling aspect ratio fix if needed")]
    public Vector2 textureScale = new Vector2(1, 1);

    void Start()
    {
        if (rend == null) rend = GetComponent<Renderer>();

        // Apply initial tiling setup
        if (rend != null)
            rend.material.mainTextureScale = textureScale;
    }

    void LateUpdate()
    {
        if (ship == null || rend == null) return;

        // Rotate to show wind direction relative to ship heading
        float relativeAngle = ship.windDirDeg - ship.headingDeg;
        transform.rotation = Quaternion.Euler(0, 0, relativeAngle);

        // Scroll speed scales with wind strength
        float currentSpeed = baseSpeed + (ship.windStrength * windMultiplier);

        Vector2 offset = rend.material.mainTextureOffset;
        offset.x += currentSpeed * Time.deltaTime;
        offset.x %= 1f;

        rend.material.mainTextureOffset = offset;
    }
}
