using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Collider2D))]
public class PlayerInteractor : MonoBehaviour
{
    public bool isPlayerOne = true;

    IInteractable current;
    Collider2D currentCol;

    void Update()
    {
        if (current == null) return;

        if (WasInteractPressed())
        {
            if (current.CanInteract(this))
            {
                current.Interact(this);
            }
        }
    }

    bool WasInteractPressed()
    {
        if (Keyboard.current == null) return false;

        if (isPlayerOne)
            return Keyboard.current.eKey.wasPressedThisFrame;
        else
            return Keyboard.current.rightShiftKey.wasPressedThisFrame;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        var interactable = other.GetComponentInParent<IInteractable>();
        if (interactable == null) return;

        current = interactable;
        currentCol = other;

        // debug
        Debug.Log($"{name} entered: {interactable.Prompt}");
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other != currentCol) return;

        // debug
        Debug.Log($"{name} exit interactable");

        current = null;
        currentCol = null;
    }
}