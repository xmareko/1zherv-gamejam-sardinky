using UnityEngine;
using UnityEngine.InputSystem;

public class WorldMover : MonoBehaviour
{
    [Header("References")]
    public ShipController ship;   // ShipRoot s ShipControllerem

    [Header("Movement")]
    public float forwardSpeed = 5f;     // když nechceš brát ship.speed, můžeš použít tohle
    public bool useShipSpeed = true;    // true = používá ship.speed

    [Header("Turning")]
    public float turnSpeed = 120f;      // deg/sec

    // Pivot: střed lodi (loď je fixně na (0,0,0))
    Vector3 pivotPoint = Vector3.zero;

    void Update()
    {
        if (ship == null) return;

        float dt = Time.deltaTime;

        // 1) HELM INPUT: jen když někdo drží kormidlo
        float steerInput = 0f;

        if (ship.helmsman != null && Keyboard.current != null)
        {
            // helmsman je ten, kdo drží helm - klávesy podle hráče
            if (ship.helmsman.isPlayerOne)
            {
                steerInput = (Keyboard.current.dKey.isPressed ? 1f : 0f) - (Keyboard.current.aKey.isPressed ? 1f : 0f);
            }
            else
            {
                steerInput = (Keyboard.current.rightArrowKey.isPressed ? 1f : 0f) - (Keyboard.current.leftArrowKey.isPressed ? 1f : 0f);
            }

            // MÍSTO přímého otáčení: upravujeme stav kormidla (-3..+3)
            ship.UpdateHelmFromInput(steerInput, dt);
        }
        else
        {
            // Pokud chceš samonávrat do středu, nastav helmReturnPerSec > 0.
            // Když helmReturnPerSec = 0, kormidlo zůstane natočené navždy.
            ship.AutoCenterHelm(dt);
        }

        // 2) TURN: loď (svět) se otáčí podle nastavení kormidla, ne podle stisknuté klávesy
        float rotationAmount = ship.helm * ship.turnPerHelmUnit * dt;

        // svět se otáčí proti lodi
        transform.RotateAround(pivotPoint, Vector3.forward, rotationAmount);

        // udrž heading synchronizovaný (užitečné pro kompas / vítr)
        ship.headingDeg -= rotationAmount;
        if (ship.headingDeg > 180f) ship.headingDeg -= 360f;
        if (ship.headingDeg < -180f) ship.headingDeg += 360f;

        // 3) FORWARD: svět teče vždy doleva PO OBRAZOVCE (Space.World)
        float spd = useShipSpeed ? ship.speed : forwardSpeed;
        transform.Translate(Vector3.left * spd * dt, Space.World);
        
        if (Time.frameCount % 30 == 0)
            Debug.Log($"helm={ship.helm:F2}");
    }
}
