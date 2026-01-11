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

        // když to už drží tenhle hráč -> pustí
        if (ship.GetCannonOperator(slot) == interactor)
        {
            ship.ClearCannonOperator(slot, interactor);
            Debug.Log($"{interactor.name} released CANNON ({slot})");
            return;
        }

        // když to drží někdo jiný -> nejde vzít
        var current = ship.GetCannonOperator(slot);
        if (current != null && current != interactor)
        {
            Debug.Log("Cannon already taken!");
            return;
        }

        // konflikty ovládání (A/D)
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

        // navíc: hráč nesmí držet jiné dělo zároveň (volitelné, ale doporučuju)
        // (tímhle zabráníš tomu, že si vezme Left a pak Front a zůstane “zaseklý”)
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
