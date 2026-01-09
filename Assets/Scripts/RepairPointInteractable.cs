using UnityEngine;
using UnityEngine.InputSystem;

public class RepairPointInteractable : MonoBehaviour, IInteractable
{
    public string Prompt => "Repair";

    public DamagePoint point;
    public float repairTime = 2.0f;

    float timer;
    PlayerInteractor repairingPlayer;
    bool ignoreCancelThisFrame;

    void Awake()
    {
        if (point == null) point = GetComponent<DamagePoint>();
    }

    public bool CanInteract(PlayerInteractor interactor)
    {
        if (point == null) return false;

        // ✅ během opravy vždy povol tomuhle hráči zrušit
        if (repairingPlayer == interactor) return true;

        // jinak musí být poškozené
        if (!point.isDamaged) return false;

        // musí mít tool
        if (RepairToolPickup.toolOwner != interactor) return false;

        // nesmí opravovat někdo jiný
        if (repairingPlayer != null && repairingPlayer != interactor) return false;

        return true;
    }

    public void Interact(PlayerInteractor interactor)
    {
        if (point == null) return;

        // Toggle OFF
        if (repairingPlayer == interactor)
        {
            CancelRepair($"{interactor.name} cancelled repair");
            return;
        }

        // Toggle ON
        if (!CanInteract(interactor))
        {
            Debug.Log("Can't repair (need tool / not damaged / busy)");
            return;
        }

        repairingPlayer = interactor;
        timer = 0f;
        ignoreCancelThisFrame = true;

        LockMovement(repairingPlayer, true);
        Debug.Log($"{repairingPlayer.name} started repairing {point.name}");
    }

    void Update()
    {
        if (repairingPlayer == null) return;

        timer += Time.deltaTime;
        if (timer >= repairTime)
        {
            point.Repair();
            FinishRepair();
        }
    }


    void CancelRepair(string reason)
    {
        Debug.Log(reason);
        FinishRepair();
    }

    void FinishRepair()
    {
        if (repairingPlayer != null)
            LockMovement(repairingPlayer, false);

        repairingPlayer = null;
        timer = 0f;
    }

    void LockMovement(PlayerInteractor player, bool locked)
    {
        var pc = player.GetComponent<PlayerController>();
        if (pc != null)
        {
            pc.enabled = !locked;
            var rb = player.GetComponent<Rigidbody2D>();
            if (rb != null) rb.linearVelocity = Vector2.zero;
        }
    }
}
