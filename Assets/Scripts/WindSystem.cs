using UnityEngine;

public class WindSystem : MonoBehaviour
{
    public ShipController ship;

    [Header("Wind Ranges")]
    public float minStrength = 0.3f;
    public float maxStrength = 1.0f;

    [Header("Direction Behavior")]
    [Tooltip("Základní (převládající) směr větru.")]
    [Range(0f, 360f)] public float baseDirDeg = 0f;

    [Tooltip("Max odchylka od base směru (±).")]
    [Range(0f, 180f)] public float dirSwingDeg = 90f;

    [Tooltip("Jak rychle se aktuální směr točí k cílovému (deg/s).")]
    public float dirTurnRateDegPerSec = 20f;

    [Header("Change Behavior (Perlin Noise)")]
    public float dirNoiseSpeed = 0.15f;
    public float strengthNoiseSpeed = 0.12f;

    [Header("Debug / Tuning")]
    public bool freezeWind = false;
    [Range(0f, 360f)] public float fixedDirDeg = 0f;
    [Range(0f, 1f)] public float fixedStrength = 1f;

    float dirSeed;
    float strengthSeed;

    float currentDirDeg; // držíme vlastní "aktuální směr", aby se točilo přes DeltaAngle

    void Awake()
    {
        dirSeed = Random.Range(0f, 1000f);
        strengthSeed = Random.Range(0f, 1000f);

        // startovní směr
        currentDirDeg = baseDirDeg;
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

        // Perlin noise -> hladké změny 0..1
        float dir01 = Mathf.PerlinNoise(dirSeed, t * dirNoiseSpeed);
        float str01 = Mathf.PerlinNoise(strengthSeed, t * strengthNoiseSpeed);

        // cílový směr jako odchylka okolo base směru
        float targetOffset = Mathf.Lerp(-dirSwingDeg, dirSwingDeg, dir01);
        float targetDirDeg = Wrap360(baseDirDeg + targetOffset);

        // plynulé natáčení směru (bez skoků přes 0/360)
        currentDirDeg = MoveAngle360(currentDirDeg, targetDirDeg, dirTurnRateDegPerSec * dt);

        // síla větru
        float strength = Mathf.Lerp(minStrength, maxStrength, str01);

        ship.windDirDeg = currentDirDeg;
        ship.windStrength = strength;
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
