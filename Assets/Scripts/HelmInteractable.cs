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

    public bool CanInteract(PlayerInteractor interactor)
    {
        return ship != null;
    }

    public void Interact(PlayerInteractor interactor)
    {
        if (ship.helmsman == interactor)
        {
            ship.ClearHelmsman(interactor);
            Debug.Log($"{interactor.name} released HELM");
        }
        else if (ship.helmsman == null)
        {
            ship.SetHelmsman(interactor);
            Debug.Log($"{interactor.name} took HELM");
        }
        else
        {
            Debug.Log("Helm is already taken!");
        }
    }
}