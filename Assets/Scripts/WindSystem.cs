using UnityEngine;

public class WindSystem : MonoBehaviour
{
    public ShipController ship;

    [Header("Wind Ranges")]
    public float minStrength = 0.3f;
    public float maxStrength = 1.0f;

    [Header("Hold Durations")]
    [Tooltip("Jak dlouho držet stejný směr větru (sekundy).")]
    public float dirHoldSeconds = 20f;

    [Tooltip("Jak dlouho držet stejnou sílu větru (sekundy).")]
    public float strengthHoldSeconds = 10f;

    [Header("Direction Step Change")]
    [Tooltip("Min/max velikost změny směru při přepnutí (stupně).")]
    public float dirStepMinDeg = 10f;

    [Tooltip("Min/max velikost změny směru při přepnutí (stupně).")]
    public float dirStepMaxDeg = 160f;

    [Header("Direction Behavior (legacy fields)")]
    [Tooltip("Základní (převládající) směr větru. (teď se používá jen jako start)")]
    [Range(0f, 360f)] public float baseDirDeg = 0f;

    [Tooltip("Max odchylka od base směru (±). (už se nepoužívá)")]
    [Range(0f, 180f)] public float dirSwingDeg = 90f;

    [Tooltip("Jak rychle se aktuální směr točí k cíli (deg/s).")]
    public float dirTurnRateDegPerSec = 20f;

    [Header("Change Behavior (Perlin Noise)")]
    public float dirNoiseSpeed = 0.15f;       // (už se nepoužívá pro směr, nechávám kvůli Inspectoru)
    public float strengthNoiseSpeed = 0.12f;

    [Header("Strength Change")]
    [Tooltip("Jak rychle se síla větru mění k cíli (za sekundu).")]
    public float strengthChangePerSec = 0.25f;

    [Header("Debug / Tuning")]
    public bool freezeWind = false;
    [Range(0f, 360f)] public float fixedDirDeg = 0f;
    [Range(0f, 1f)] public float fixedStrength = 1f;

    float dirSeed;
    float strengthSeed;

    float currentDirDeg;
    float targetDirDeg;
    float dirTimer;

    float currentStrength;
    float targetStrength;
    float strengthTimer;

    void Awake()
    {
        dirSeed = Random.Range(0f, 1000f);
        strengthSeed = Random.Range(0f, 1000f);

        // start
        currentDirDeg = Wrap360(baseDirDeg);
        targetDirDeg = currentDirDeg;
        dirTimer = 0f;

        currentStrength = Mathf.Clamp01(maxStrength);
        targetStrength = currentStrength;
        strengthTimer = 0f;
    }

    void Update()
    {
        if (ship == null) return;

        if (freezeWind)
        {
            ship.windDirDeg = fixedDirDeg;
            ship.windStrength = fixedStrength;
            return;
        }

        float t = Time.time;
        float dt = Time.deltaTime;

        // --- Direction: pick new target every dirHoldSeconds ---
        dirTimer -= dt;
        if (dirTimer <= 0f)
        {
            float minStep = Mathf.Clamp(dirStepMinDeg, 0f, 180f);
            float maxStep = Mathf.Clamp(dirStepMaxDeg, minStep, 180f);

            float step = Random.Range(minStep, maxStep);
            float sign = Random.value < 0.5f ? -1f : 1f;

            targetDirDeg = Wrap360(currentDirDeg + sign * step);

            dirTimer = Mathf.Max(0.1f, dirHoldSeconds);
        }

        // --- Strength: pick new target every strengthHoldSeconds ---
        strengthTimer -= dt;
        if (strengthTimer <= 0f)
        {
            float str01 = Mathf.PerlinNoise(strengthSeed, t * strengthNoiseSpeed);
            targetStrength = Mathf.Lerp(minStrength, maxStrength, str01);

            strengthTimer = Mathf.Max(0.1f, strengthHoldSeconds);
        }

        // --- Smoothly move to targets ---
        currentDirDeg = MoveAngle360(currentDirDeg, targetDirDeg, dirTurnRateDegPerSec * dt);
        currentStrength = Mathf.MoveTowards(currentStrength, targetStrength, strengthChangePerSec * dt);

        ship.windDirDeg = currentDirDeg;
        ship.windStrength = currentStrength;
    }

    static float MoveAngle360(float current, float target, float maxDelta)
    {
        float delta = Mathf.DeltaAngle(current, target);
        if (Mathf.Abs(delta) <= maxDelta) return Wrap360(target);
        return Wrap360(current + Mathf.Sign(delta) * maxDelta);
    }

    static float Wrap360(float deg)
    {
        deg %= 360f;
        if (deg < 0f) deg += 360f;
        return deg;
    }
}
