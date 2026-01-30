using UnityEngine;
using UnityEngine.UI;

public class InteractHintUI : MonoBehaviour
{
    [Header("UI")]
    public RectTransform root;
    public Image icon;

    [Header("Sprites")]
    public Sprite pressE;
    public Sprite pressShift;

    [Header("Positioning")]
    public Vector3 worldOffset = new Vector3(0f, 1.2f, 0f);

    Camera cam;
    Transform target;

    void Awake()
    {
        cam = Camera.main;

        if (root != null)
            root.gameObject.SetActive(false);
    }

    void LateUpdate()
    {
        if (target == null || root == null || cam == null) return;

        Vector3 worldPos = target.position + worldOffset;
        Vector3 screenPos = cam.WorldToScreenPoint(worldPos);

        bool behind = screenPos.z < 0f;
        root.gameObject.SetActive(!behind);

        if (!behind)
            root.position = screenPos;
    }

    public void Show(Transform worldTarget, bool isPlayerOne)
    {
        target = worldTarget;

        if (icon != null)
            icon.sprite = isPlayerOne ? pressE : pressShift;

        if (root != null)
            root.gameObject.SetActive(true);
    }

    public void Hide()
    {
        target = null;

        if (root != null)
            root.gameObject.SetActive(false);
    }
}