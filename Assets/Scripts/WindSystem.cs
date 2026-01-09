using UnityEngine;

public class WindSystem : MonoBehaviour
{
    public ShipController ship;

    [Header("Wind Ranges")]
    public float minStrength = 0.3f;
    public float maxStrength = 1.0f;

    [Header("Change Behavior (Perlin Noise)")]
    public float dirNoiseSpeed = 0.15f;
    public float strengthNoiseSpeed = 0.12f;

    [Header("Debug / Tuning")]
    public bool freezeWind = false;
    [Range(0f, 360f)] public float fixedDirDeg = 0f;
    [Range(0f, 1f)] public float fixedStrength = 1f;

    float dirSeed;
    float strengthSeed;

    void Awake()
    {
        dirSeed = Random.Range(0f, 1000f);
        strengthSeed = Random.Range(0f, 1000f);
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

        // Perlin noise -> hladké změny
        float dir01 = Mathf.PerlinNoise(dirSeed, t * dirNoiseSpeed);
        float str01 = Mathf.PerlinNoise(strengthSeed, t * strengthNoiseSpeed);

        // 0..360 je pro UI i debug přehlednější než -180..180
        ship.windDirDeg = Mathf.Lerp(0f, 360f, dir01);
        ship.windStrength = Mathf.Lerp(minStrength, maxStrength, str01);
    }
}