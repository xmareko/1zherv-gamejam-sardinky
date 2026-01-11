using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float lifeTime = 2.5f;

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }
}