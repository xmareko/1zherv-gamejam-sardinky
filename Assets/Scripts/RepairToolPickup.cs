using UnityEngine;

public class RepairToolPickup : MonoBehaviour, IInteractable
{
    public string Prompt => (toolOwner == null) ? "Take Repair Tool" : "Return Repair Tool";

    public static PlayerInteractor toolOwner; // jen jeden hráč může mít tool

    [Header("Visual")]
    public GameObject toolVisual; // sprite/model toolu na zemi (volitelné)

    void Awake()
    {
        // ať se tool vždy resetne po startu scény
        toolOwner = null;
        UpdateVisual();
    }

    public bool CanInteract(PlayerInteractor interactor)
    {
        // může vzít, pokud nikdo nemá
        if (toolOwner == null) return true;

        // může vrátit jen ten, kdo ho drží
        return toolOwner == interactor;
    }

    public void Interact(PlayerInteractor interactor)
    {
        if (toolOwner == null)
        {
            // TAKE
            toolOwner = interactor;
            Debug.Log($"{interactor.name} took the REPAIR TOOL");
            UpdateVisual();
            return;
        }

        if (toolOwner == interactor)
        {
            // RETURN
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

        // pokud nemáš toolVisual, můžeš rovnou skrývat celý objekt:
        // gameObject.SetActive(toolOwner == null);
        // (ale pak by nešlo tool vracet přes stejný pickup, proto je lepší toolVisual)
    }
}