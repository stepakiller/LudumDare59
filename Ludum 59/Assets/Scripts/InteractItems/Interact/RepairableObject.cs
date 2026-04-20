using UnityEngine;
using System;

public class RepairableObject : MonoBehaviour, Interactable
{
    public event Action OnRepaired;
    public event Action OnBroken;
    [SerializeField] Behaviour breakdownScript;
    [SerializeField] float repairDuration = 2f;
    [SerializeField] ItemData requiredItem;
    public float RepairDuration => repairDuration;
    public ItemData RequiredItem => requiredItem;
    public bool IsRepaired { get; private set; } = true; 

    public void Interact()
    {
        if (!IsRepaired)
        {
            ItemData heldItem = Bootstrapper.Inventory?.CurrentItem?.ItemData;
            
            if (requiredItem != null && heldItem != requiredItem)
            {
                Debug.Log($"Для починки нужен предмет: {requiredItem.ItemName}!");
                // Здесь в будущем можно вызвать событие для показа UI-уведомления
                return;
            }
            RepairComplete();
        }
    }
    public void Break()
    {
        if (!IsRepaired) return;
        IsRepaired = false;
        if (breakdownScript != null) breakdownScript.enabled = true;
        OnBroken?.Invoke();
    }

    public void RepairComplete()
    {
        if (IsRepaired) return;
        IsRepaired = true;
        if (breakdownScript != null) breakdownScript.enabled = false;
        OnRepaired?.Invoke();
    }
}