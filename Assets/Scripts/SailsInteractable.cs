using UnityEngine;

public class SailsInteractable : MonoBehaviour, IInteractable
{
    public string Prompt => "Use Sails";

    ShipController ship;

    void Awake()
    {
        ship = GetComponentInParent<ShipController>();
        if (ship == null) Debug.LogError("SailsInteractable: ShipController not found!");
    }

    public bool CanInteract(PlayerInteractor interactor) => ship != null;

    public void Interact(PlayerInteractor interactor)
    {
        if (ship.sailOperator == interactor)
        {
            ship.ClearSailOperator(interactor);
            Debug.Log($"{interactor.name} released SAILS");
        }
        else if (ship.sailOperator == null)
        {
            ship.SetSailOperator(interactor);
            Debug.Log($"{interactor.name} took SAILS");
        }
        else
        {
            Debug.Log("Sails already taken!");
        }
    }
}