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
        if (ship == null) return;

        // Když to už drží tenhle hráč -> pustí
        if (ship.sailOperator == interactor)
        {
            ship.ClearSailOperator(interactor);
            Debug.Log($"{interactor.name} released SAILS");
            return;
        }

        // Když plachty drží někdo jiný -> nejde vzít
        if (ship.sailOperator != null && ship.sailOperator != interactor)
        {
            Debug.Log("Sails already taken!");
            return;
        }

        // DŮLEŽITÉ: stejný hráč nesmí držet helm i plachty (A/D konflikt)
        if (ship.helmsman == interactor)
        {
            Debug.Log("You are at the HELM. Another player must handle SAILS.");
            return;
        }

        // Jinak: může vzít plachty, i když druhý hráč drží kormidlo
        ship.SetSailOperator(interactor);
        Debug.Log($"{interactor.name} took SAILS");
    }
}