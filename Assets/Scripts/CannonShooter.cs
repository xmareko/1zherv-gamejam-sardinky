using System.Collections;
using UnityEngine;

public class CannonShooter : MonoBehaviour
{
    [Header("Projectile")]
    public GameObject projectilePrefab;
    public Transform muzzle;         // odkud střela vyletí
    public float shotSpeed = 12f;
    public float spawnOffset = 0.1f;

    [Header("Reload")]
    public float reloadTime = 1.2f;
    public SpriteRenderer loadedBallSprite; // sprite “koule v hlavni”

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

        // 1) “vystřelit” – dělo se vyprázdní
        loaded = false;
        SetLoadedVisual(false);

        // 2) spawn projektilu
        Vector3 spawnPos = muzzle.position + muzzle.right * spawnOffset;
        GameObject go = Instantiate(projectilePrefab, spawnPos, muzzle.rotation);

        // 3) dát mu rychlost směrem kam míří dělo
        var rb = go.GetComponent<Rigidbody2D>();
        if (rb != null)
            rb.linearVelocity = (Vector2)muzzle.right * shotSpeed;

        // 4) reload
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