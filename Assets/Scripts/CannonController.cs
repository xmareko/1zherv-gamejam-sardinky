using UnityEngine;

public class CannonController : MonoBehaviour
{
    public Transform rotatePivot;
    public float rotateSpeedDegPerSec = 120f;
    public float maxAngle = 60f;

    float baseLocalZ;

    void Awake()
    {
        if (rotatePivot == null) rotatePivot = transform;
        baseLocalZ = rotatePivot.localEulerAngles.z;
    }

    public void Rotate(float input, float dt)
    {
        float currentZ = rotatePivot.localEulerAngles.z;
        float delta = Mathf.DeltaAngle(baseLocalZ, currentZ);

        delta += input * rotateSpeedDegPerSec * dt;
        delta = Mathf.Clamp(delta, -maxAngle, maxAngle);

        rotatePivot.localRotation = Quaternion.Euler(0f, 0f, baseLocalZ + delta);
    }
}