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

        // Pivot drž ve středu lodi (loď je fixně na 0,0)
        worldPivot.position = Vector3.zero;

        // Otáčej pivot kolem lodi
        worldPivot.rotation = Quaternion.Euler(0, 0, -ship.headingDeg);

        // Posouvej OBSAH světa proti směru "dopředu" lodi
        // Loď je vizuálně doprava => svět jde doleva v lokálním směru pivotu
        worldContent.position -= worldPivot.right * (ship.speed * dt);
    }
}