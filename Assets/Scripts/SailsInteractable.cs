using UnityEngine;

public class SailsInteractable : MonoBehaviour, IInteractable
{
    public string Prompt => "Use Sails";

    ShipController ship;

    void Awake()
    {
        ship = GetComponentInParent<ShipController>();
        if (ship == null)
            Debug.LogError("SailsInteractable: ShipController not found!");
    }

    public bool CanInteract(PlayerInteractor interactor) => ship != null;

    public void Interact(PlayerInteractor interactor)
    {
        if (ship == null) return;

        // Release if the same player is already operating sails
        if (ship.sailOperator == interactor)
        {
            ship.ClearSailOperator(interactor);
            Debug.Log($"{interactor.name} released SAILS");
            return;
        }

        // Prevent takeover by another player
        if (ship.sailOperator != null && ship.sailOperator != interactor)
        {
            Debug.Log("Sails already taken!");
            return;
        }

        // Prevent helm + sails control conflict
        if (ship.helmsman == interactor)
        {
            Debug.Log("You are at the HELM. Another player must handle SAILS.");
            return;
        }

        ship.SetSailOperator(interactor);
        Debug.Log($"{interactor.name} took SAILS");
    }
}
