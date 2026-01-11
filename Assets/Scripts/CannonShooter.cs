using System.Collections;
using UnityEngine;

public class CannonShooter : MonoBehaviour
{
    [Header("Projectile")]
    public GameObject projectilePrefab;
    public Transform muzzle;
    public float shotSpeed = 12f;
    public float spawnOffset = 0.1f;

    [Header("Reload")]
    public float reloadTime = 1.2f;
    public SpriteRenderer loadedBallSprite;

    bool loaded = true;
    Coroutine reloadRoutine;

    void Start()
    {
        SetLoadedVisual(true);
    }

    public bool CanShoot()
    {
        return loaded && projectilePrefab != null && muzzle != null;
    }

    public void Shoot()
    {
        if (!CanShoot()) return;

        // boom
        SimpleAudio.Instance.PlayShoot();

        // Consume loaded state
        loaded = false;
        SetLoadedVisual(false);

        Vector3 spawnPos = muzzle.position + muzzle.right * spawnOffset;
        GameObject go = Instantiate(projectilePrefab, spawnPos, muzzle.rotation);

        var rb = go.GetComponent<Rigidbody2D>();
        if (rb != null)
            rb.linearVelocity = (Vector2)muzzle.right * shotSpeed;

        if (reloadRoutine != null) StopCoroutine(reloadRoutine);
        reloadRoutine = StartCoroutine(ReloadAfterDelay());
    }

    IEnumerator ReloadAfterDelay()
    {
        yield return new WaitForSeconds(reloadTime);
        loaded = true;
        SetLoadedVisual(true);
        reloadRoutine = null;
    }

    void SetLoadedVisual(bool isLoaded)
    {
        if (loadedBallSprite != null)
            loadedBallSprite.enabled = isLoaded;
    }
}
