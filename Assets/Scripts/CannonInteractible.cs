using UnityEngine;

public class CannonInteractable : MonoBehaviour, IInteractable
{
    public string Prompt => "Use Cannon";

    public CannonSlot slot = CannonSlot.Left;
    public CannonController cannon;

    ShipController ship;

    void Awake()
    {
        ship = GetComponentInParent<ShipController>();
        if (ship == null) Debug.LogError("CannonInteractable: ShipController not found!");

        if (cannon == null)
            cannon = GetComponentInChildren<CannonController>();

        if (cannon == null)
            Debug.LogError("CannonInteractable: CannonController not found!");
    }

    public bool CanInteract(PlayerInteractor interactor) => ship != null && cannon != null;

    public void Interact(PlayerInteractor interactor)
    {
        if (ship == null || cannon == null) return;

        // Release if the same player is already operating this cannon
        if (ship.GetCannonOperator(slot) == interactor)
        {
            ship.ClearCannonOperator(slot, interactor);
            Debug.Log($"{interactor.name} released CANNON ({slot})");
            return;
        }

        // Prevent takeover by another player
        var current = ship.GetCannonOperator(slot);
        if (current != null && current != interactor)
        {
            Debug.Log("Cannon already taken!");
            return;
        }

        // Prevent control conflicts
        if (ship.helmsman == interactor)
        {
            Debug.Log("You are at the HELM. Another player must handle the CANNON.");
            return;
        }
        if (ship.sailOperator == interactor)
        {
            Debug.Log("You are on SAILS. Another player must handle the CANNON.");
            return;
        }

        // Player cannot operate multiple cannons at once
        if (ship.leftCannonOperator == interactor ||
            ship.rightCannonOperator == interactor ||
            ship.frontCannonOperator == interactor)
        {
            Debug.Log("You are already operating a cannon.");
            return;
        }

        ship.SetCannonOperator(slot, interactor, cannon);
        Debug.Log($"{interactor.name} took CANNON ({slot})");
    }
}

public enum CannonSlot
{
    Left,
    Right,
    Front
}
