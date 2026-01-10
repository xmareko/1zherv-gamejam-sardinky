using UnityEngine;

public class WaterBackground : MonoBehaviour
{
    public ShipController ship;
    public Renderer rend;

    void LateUpdate()
    {
        if (ship == null || rend == null) return;

        // 1. ROTATION
        // Rotate the Water Quad to match the World Pivot
        transform.rotation = Quaternion.Euler(0, 0, -ship.headingDeg);

        // 2. SCROLLING
        // We want the texture to visually move Left (Screen West).
        // In Unity, INCREASING the Texture Offset moves the image Left.
        // So we calculate the vector for "World Right" relative to our rotated quad.
        Vector3 scrollDirWorld = Vector3.right;

        // Convert "World Right" into the Quad's Local Space
        Vector3 scrollDirLocal = transform.InverseTransformDirection(scrollDirWorld);

        // Calculate scale ratio
        float quadWidthWorld = transform.lossyScale.x;
        float textureRepeats = rend.material.mainTextureScale.x;
        float uvPerMeter = textureRepeats / quadWidthWorld;

        // Calculate shift
        Vector2 offsetChange = (Vector2)scrollDirLocal * ship.speed * Time.deltaTime * uvPerMeter;

        // 3. APPLY (Add the change)
        rend.material.mainTextureOffset += offsetChange;
    }
}