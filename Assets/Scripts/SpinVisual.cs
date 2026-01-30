using UnityEngine;

public class SpinVisual : MonoBehaviour
{
    public float speedDegPerSec = 180f;

    void Update()
    {
        transform.Rotate(0f, 0f, speedDegPerSec * Time.deltaTime);
    }
}