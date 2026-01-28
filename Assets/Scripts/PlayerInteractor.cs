using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Collider2D))]
public class PlayerInteractor : MonoBehaviour
{
    public bool isPlayerOne = true;

    [Header("Visuals")]
    [Tooltip("The GameObject (child of this player) that represents the held tool.")]
    public GameObject heldToolVisual;

    IInteractable current;
    Collider2D currentCol;

    void Start()
    {
        // Ensure the tool is hidden when the game starts
        if (heldToolVisual != null)
            heldToolVisual.SetActive(false);
    }

    void Update()
    {
        if (current == null) return;

        if (WasInteractPressed() && current.CanInteract(this))
            current.Interact(this);
    }

    bool WasInteractPressed()
    {
        if (Keyboard.current == null) return false;

        return isPlayerOne
            ? Keyboard.current.eKey.wasPressedThisFrame
            : Keyboard.current.rightShiftKey.wasPressedThisFrame;
    }

    // --- NEW METHOD ---
    public void SetHoldingTool(bool holding)
    {
        if (heldToolVisual != null)
            heldToolVisual.SetActive(holding);
    }
    // ------------------

    void OnTriggerEnter2D(Collider2D other)
    {
        var interactable = other.GetComponentInParent<IInteractable>();
        if (interactable == null) return;

        current = interactable;
        currentCol = other;

        Debug.Log($"{name} entered: {interactable.Prompt}");
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other != currentCol) return;

        Debug.Log($"{name} exit interactable");

        current = null;
        currentCol = null;
    }
}