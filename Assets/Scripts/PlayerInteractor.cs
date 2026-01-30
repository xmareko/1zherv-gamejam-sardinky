using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Collider2D))]
public class PlayerInteractor : MonoBehaviour
{
    public bool isPlayerOne = true;

    [Header("UI")]
    public InteractHintUI hintUI;

    [Header("Visuals")]
    [Tooltip("The GameObject (child of this player) that represents the held tool.")]
    public GameObject heldToolVisual;

    IInteractable current;
    Collider2D currentCol;
    PlayerController pc;

    void Awake()
    {
        pc = GetComponent<PlayerController>();
    }

    void Start()
    {
        if (heldToolVisual != null)
            heldToolVisual.SetActive(false);

        if (hintUI != null)
            hintUI.Hide();
    }

    void Update()
    {
        if (current == null) return;

        RefreshHint();

        if (WasInteractPressed() && current.CanInteract(this))
            current.Interact(this);
    }

    void RefreshHint()
    {
        if (hintUI == null) return;

        // If player movement is disabled, they are operating something (helm/sails/cannon/repair)
        if (pc != null && !pc.enabled)
        {
            hintUI.Hide();
            return;
        }

        if (currentCol == null)
        {
            hintUI.Hide();
            return;
        }

        if (current.CanInteract(this))
            hintUI.Show(currentCol.transform, isPlayerOne);
        else
            hintUI.Hide();
    }

    bool WasInteractPressed()
    {
        if (Keyboard.current == null) return false;

        return isPlayerOne
            ? Keyboard.current.eKey.wasPressedThisFrame
            : Keyboard.current.rightShiftKey.wasPressedThisFrame;
    }

    public void SetHoldingTool(bool holding)
    {
        if (heldToolVisual != null)
            heldToolVisual.SetActive(holding);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        var interactable = other.GetComponentInParent<IInteractable>();
        if (interactable == null) return;

        current = interactable;
        currentCol = other;

        RefreshHint();
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other != currentCol) return;

        current = null;
        currentCol = null;

        if (hintUI != null)
            hintUI.Hide();
    }
}
