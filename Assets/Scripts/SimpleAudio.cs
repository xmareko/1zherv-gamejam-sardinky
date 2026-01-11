using UnityEngine;

public class SimpleAudio : MonoBehaviour
{
    public static SimpleAudio Instance;

    [Header("Audio Sources")]
    public AudioSource sfxSource;
    public AudioSource windSource;

    [Header("Audio Clips")]
    public AudioClip shootClip;
    public AudioClip winClip;
    public AudioClip gameOverClip;

    [Header("Settings")]
    public float currentWindSpeed;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        windSource.volume = Mathf.Clamp01(currentWindSpeed / 100f);
    }

    public void PlayShoot()
    {
        sfxSource.PlayOneShot(shootClip);
    }

    public void PlayWin()
    {
        sfxSource.PlayOneShot(winClip);
    }

    public void PlayGameOver()
    {
        sfxSource.PlayOneShot(gameOverClip);
    }
}