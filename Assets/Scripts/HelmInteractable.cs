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

        // Pokud helm drží tento hráč -> pustí
        if (ship.helmsman == interactor)
        {
            ship.ClearHelmsman(interactor);
            Debug.Log($"{interactor.name} released HELM");
            return;
        }

        // Pokud helm drží někdo jiný -> nejde vzít
        if (ship.helmsman != null && ship.helmsman != interactor)
        {
            Debug.Log("Helm is already taken!");
            return;
        }

        // DŮLEŽITÉ: stejný hráč nesmí držet helm i plachty (A/D konflikt)
        if (ship.sailOperator == interactor)
        {
            Debug.Log("You are handling SAILS. Another player must take the HELM.");
            return;
        }

        // Jinak: může vzít helm, i když druhý hráč drží plachty
        ship.SetHelmsman(interactor);
        Debug.Log($"{interactor.name} took HELM");
    }
}