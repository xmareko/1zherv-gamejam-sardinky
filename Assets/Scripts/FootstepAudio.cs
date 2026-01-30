using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(AudioSource))]
public class FootstepAudio : MonoBehaviour
{
    public float minSpeedToPlay = 0.1f;
    public float fadeInSpeed = 10f;
    public float fadeOutSpeed = 10f;

    Rigidbody2D rb;
    AudioSource src;
    float targetVolume;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        src = GetComponent<AudioSource>();

        src.playOnAwake = false;
        src.loop = true;

        targetVolume = src.volume;
        src.volume = 0f;
    }

    void Update()
    {
        float speed = rb.linearVelocity.magnitude;
        bool shouldPlay = speed > minSpeedToPlay;

        if (shouldPlay)
        {
            if (!src.isPlaying) src.Play();
            src.volume = Mathf.MoveTowards(src.volume, targetVolume, fadeInSpeed * Time.deltaTime);
        }
        else
        {
            src.volume = Mathf.MoveTowards(src.volume, 0f, fadeOutSpeed * Time.deltaTime);
            if (src.isPlaying && src.volume <= 0.001f)
                src.Stop();
        }
    }
}