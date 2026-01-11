using UnityEngine;

public class HelmInteractable : MonoBehaviour, IInteractable
{
    public string Prompt => "Use Helm";

    ShipController ship;

    void Awake()
    {
        ship = GetComponentInParent<ShipController>();
        if (ship == null)
            Debug.LogError("HelmInteractable: ShipController not found in parents!");
    }

    public bool CanInteract(PlayerInteractor interactor) => ship != null;

    public void Interact(PlayerInteractor interactor)
    {
        if (ship == null) return;

        // Release if the same player is already helmsman
        if (ship.helmsman == interactor)
        {
            ship.ClearHelmsman(interactor);
            Debug.Log($"{interactor.name} released HELM");
            return;
        }

        // Prevent takeover by another player
        if (ship.helmsman != null && ship.helmsman != interactor)
        {
            Debug.Log("Helm is already taken!");
            return;
        }

        // Prevent helm + sails control conflict
        if (ship.sailOperator == interactor)
        {
            Debug.Log("You are handling SAILS. Another player must take the HELM.");
            return;
        }

        ship.SetHelmsman(interactor);
        Debug.Log($"{interactor.name} took HELM");
    }
}
