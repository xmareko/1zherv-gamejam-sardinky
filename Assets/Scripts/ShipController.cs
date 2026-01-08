using UnityEngine;
using UnityEngine.InputSystem;


public class ShipController : MonoBehaviour
{
    [Header("Sails")]
    public float sailTrim = 0f;           // aktuální trim (např. -1..+1)
    public float sailTrimMax = 1f;        // rozsah trimu
    public float sailChangePerSec = 1.5f; // jak rychle hráč nastavuje plachty
    public PlayerInteractor sailOperator; // kdo právě ovládá plachty (null = nikdo)

    [Header("Wind")]
    public float windDirDeg = 0f;         // směr větru ve světě (0 = doprava)
    public float windStrength = 1f;       // 0..1
    public float windDirChangePerSec = 8f;// jak rychle se vítr mění (deg/sec) - náhodně
    public float windStrengthChangePerSec = 0.2f; // jak rychle se mění síla

    
    [Header("Helm")]
    public float helm = 0f;              // aktuální nastavení kormidla
    public float helmMax = 0.5f;           // -0.5..+0.5
    public float helmChangePerSec = 1.5f; // jak rychle hráč točí kormidlem (jednotky za vteřinu)
    public float helmReturnPerSec = 0.2f; // kolik se samo vrací k nule (0 = nikdy)
    public float turnPerHelmUnit = 25f;   // kolik stupňů/s udělá 1 jednotka kormidla

    
    [Header("Ship State")]
    public float headingDeg = 0f;     // kam loď míří (logicky)
    public float speed = 2f;          // zatím konstanta, vítr přijde v dalším kroku

    [Header("Turning")]
    public float turnRateDegPerSec = 90f;

    public PlayerInteractor helmsman; // kdo právě řídí (null = nikdo)

    public void SetHelmsman(PlayerInteractor interactor)
    {
        helmsman = interactor;

        // zablokuj pohyb postavy
        var pc = interactor.GetComponent<PlayerController>();
        if (pc != null)
        {
            pc.enabled = false;

            // jistota: zastav rychlost RB, aby "nedojel"
            var rb = interactor.GetComponent<Rigidbody2D>();
            if (rb != null) rb.linearVelocity = Vector2.zero;
        }
    }

    public void ClearHelmsman(PlayerInteractor interactor)
    {
        if (helmsman != interactor)
            return;

        // povol pohyb postavy
        var pc = interactor.GetComponent<PlayerController>();
        if (pc != null)
            pc.enabled = true;

        helmsman = null;
    }


    public void ApplyTurnInput(float turnInput, float dt)
    {
        // turnInput -1..+1
        headingDeg += turnInput * turnRateDegPerSec * dt;

        // udržet v rozumném rozsahu
        if (headingDeg > 180f) headingDeg -= 360f;
        if (headingDeg < -180f) headingDeg += 360f;
    }

    public Vector2 ForwardWorld()
    {
        float rad = headingDeg * Mathf.Deg2Rad;
        return new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));
    }
    
    public void UpdateHelmFromInput(float steerInput, float dt)
    {
        // steerInput je -1..+1 (A/D nebo šipky)
        // přičítáme pomalu, aby to působilo jako skutečné kormidlo
        helm += steerInput * helmChangePerSec * dt;
        helm = Mathf.Clamp(helm, -helmMax, helmMax);
    }

    public void AutoCenterHelm(float dt)
    {
        // pokud chceš, aby se samo vracelo k 0 (jako některé hry),
        // nastav helmReturnPerSec > 0
        if (helmReturnPerSec <= 0f) return;

        helm = Mathf.MoveTowards(helm, 0f, helmReturnPerSec * dt);
    }
    
    public void SetSailOperator(PlayerInteractor interactor)
    {
        sailOperator = interactor;

        var pc = interactor.GetComponent<PlayerController>();
        if (pc != null)
        {
            pc.enabled = false;
            var rb = interactor.GetComponent<Rigidbody2D>();
            if (rb != null) rb.linearVelocity = Vector2.zero;
        }
    }

    public void ClearSailOperator(PlayerInteractor interactor)
    {
        if (sailOperator != interactor) return;

        var pc = interactor.GetComponent<PlayerController>();
        if (pc != null) pc.enabled = true;

        sailOperator = null;
    }

    public void UpdateSailsFromInput(float input, float dt)
    {
        sailTrim += input * sailChangePerSec * dt;
        sailTrim = Mathf.Clamp(sailTrim, -sailTrimMax, sailTrimMax);
    }


}