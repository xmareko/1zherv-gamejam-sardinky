using UnityEngine;

public class RepairPointInteractable : MonoBehaviour, IInteractable
{
    public string Prompt => "Repair";

    public DamagePoint point;
    public float repairTime = 2.0f;

    [Header("Visual")]
    public GameObject repairIndicator;

    float timer;
    PlayerInteractor repairingPlayer;

    void Awake()
    {
        if (point == null) point = GetComponent<DamagePoint>();

        if (repairIndicator != null)
            repairIndicator.SetActive(false);
    }

    public bool CanInteract(PlayerInteractor interactor)
    {
        if (point == null) return false;

        if (repairingPlayer == interactor) return true;
        if (!point.isDamaged) return false;
        if (RepairToolPickup.toolOwner != interactor) return false;
        if (repairingPlayer != null && repairingPlayer != interactor) return false;

        return true;
    }

    public void Interact(PlayerInteractor interactor)
    {
        if (point == null) return;

        if (repairingPlayer == interactor)
        {
            CancelRepair($"{interactor.name} cancelled repair");
            return;
        }

        if (!CanInteract(interactor))
        {
            Debug.Log("Can't repair (need tool / not damaged / busy)");
            return;
        }

        repairingPlayer = interactor;
        timer = 0f;

        SetRepairVisual(true);

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
        SetRepairVisual(false);

        if (repairingPlayer != null)
            LockMovement(repairingPlayer, false);

        repairingPlayer = null;
        timer = 0f;
    }

    void SetRepairVisual(bool active)
    {
        if (repairIndicator != null)
            repairIndicator.SetActive(active);
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
