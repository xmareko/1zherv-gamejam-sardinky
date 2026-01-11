using UnityEngine;

public class WaterBackground : MonoBehaviour
{
    public ShipController ship;
    public Renderer rend;

    void LateUpdate()
    {
        if (ship == null || rend == null) return;

        // Keep water aligned with ship heading while the texture scrolls in world-space
        transform.rotation = Quaternion.Euler(0, 0, -ship.headingDeg);

        Vector3 scrollDirWorld = Vector3.right;
        Vector3 scrollDirLocal = transform.InverseTransformDirection(scrollDirWorld);

        float quadWidthWorld = transform.lossyScale.x;
        float textureRepeats = rend.material.mainTextureScale.x;
        float uvPerMeter = textureRepeats / quadWidthWorld;

        Vector2 offsetChange = (Vector2)scrollDirLocal * ship.speed * Time.deltaTime * uvPerMeter;
        rend.material.mainTextureOffset += offsetChange;
    }
}
