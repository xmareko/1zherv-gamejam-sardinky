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

    public bool CanInteract(PlayerInteractor interactor)
    {
        if (ship == null) return false;
        if (ship.sailOperator == null) return true;
        return ship.sailOperator == interactor;
    }

    public void Interact(PlayerInteractor interactor)
    {
        if (ship == null) return;

        if (ship.sailOperator == interactor)
        {
            ship.ClearSailOperator(interactor);
            Debug.Log($"{interactor.name} released SAILS");
            return;
        }

        if (ship.sailOperator != null && ship.sailOperator != interactor)
        {
            Debug.Log("Sails already taken!");
            return;
        }

        if (ship.helmsman == interactor)
        {
            Debug.Log("You are at the HELM. Another player must handle SAILS.");
            return;
        }

        ship.SetSailOperator(interactor);
        Debug.Log($"{interactor.name} took SAILS");
    }
}