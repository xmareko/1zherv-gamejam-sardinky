using UnityEngine;

public class WorldController : MonoBehaviour
{
    public ShipController ship;
    public Transform worldPivot;
    public Transform worldContent;

    void LateUpdate()
    {
        if (ship == null || worldPivot == null || worldContent == null) return;

        float dt = Time.deltaTime;

        // Keep pivot centered on the ship (ship stays at origin)
        worldPivot.position = Vector3.zero;

        // Rotate the world around the ship based on heading
        worldPivot.rotation = Quaternion.Euler(0, 0, -ship.headingDeg);

        // Move world content to simulate forward ship movement
        worldContent.position -= Vector3.right * (ship.speed * dt);
    }
}
