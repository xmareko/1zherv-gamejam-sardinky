using UnityEngine;

public class WindSystem : MonoBehaviour
{
    public ShipController ship;

    [Header("Wind Ranges")]
    public float minStrength = 0.3f;
    public float maxStrength = 1.0f;

    [Header("Change Behavior")]
    public float dirNoiseSpeed = 0.15f;
    public float strengthNoiseSpeed = 0.12f;

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

        float t = Time.time;

        // Perlin noise -> hladké změny
        float dir01 = Mathf.PerlinNoise(dirSeed, t * dirNoiseSpeed);
        float str01 = Mathf.PerlinNoise(strengthSeed, t * strengthNoiseSpeed);

        ship.windDirDeg = Mathf.Lerp(-180f, 180f, dir01);
        ship.windStrength = Mathf.Lerp(minStrength, maxStrength, str01);
    }
}