using UnityEngine;

public class RepairToolPickup : MonoBehaviour, IInteractable
{
    public string Prompt => (toolOwner == null) ? "Take Repair Tool" : "Return Repair Tool";

    public static PlayerInteractor toolOwner;

    [Header("Visual")]
    public GameObject toolVisual;

    void Awake()
    {
        // Reset tool state on scene start
        toolOwner = null;
        UpdateVisual();
    }

    public bool CanInteract(PlayerInteractor interactor)
    {
        if (toolOwner == null) return true;
        return toolOwner == interactor;
    }

    public void Interact(PlayerInteractor interactor)
    {
        if (toolOwner == null)
        {
            toolOwner = interactor;
            Debug.Log($"{interactor.name} took the REPAIR TOOL");
            UpdateVisual();
            return;
        }

        if (toolOwner == interactor)
        {
            toolOwner = null;
            Debug.Log($"{interactor.name} returned the REPAIR TOOL");
            UpdateVisual();
            return;
        }

        Debug.Log("Tool already taken!");
    }

    void UpdateVisual()
    {
        if (toolVisual != null)
            toolVisual.SetActive(toolOwner == null);
    }
}
