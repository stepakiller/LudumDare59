using UnityEngine;

public class RepairableObject : MonoBehaviour, Interactable
{
    [SerializeField] Behaviour breakdownScript;
    [SerializeField] float repairDuration = 2f;
    [SerializeField] ItemData requiredItem;

    [HideInInspector] public bool isRepaired = false;

    public float RepairDuration => repairDuration;
    public ItemData RequiredItem => requiredItem;

    public void Interact()
    {
        if (!isRepaired)
        {
            ItemData heldItem = Bootstrapper.Inventory?.CurrentItem?.ItemData;
            if (requiredItem != null && heldItem != requiredItem)
            {
                Debug.Log($"Для починки нужен предмет: {requiredItem.ItemName}!");
                //добавить вывод текста на ui "вам нужна отвертка"
            }
        }
    }

    public void RepairComplete()
    {
        if (isRepaired) return;
        isRepaired = true;
        if (breakdownScript != null)  breakdownScript.enabled = false;
    }
}