using UnityEngine;
using System.Collections.Generic;
using System;

public class HotbarManager : MonoBehaviour
{
    [SerializeField] int slotCount = 3;
    public ItemSettings[] slots;
    public List<ItemSettings> radialItems = new List<ItemSettings>(); 

    public int currentSelectedIndex { get; private set; } = 0;
    public event Action OnInventoryChanged;

    void Awake() 
    {
        Bootstrapper.HotbarManager = this;
        slots = new ItemSettings[slotCount];
    }

    void Start() 
    {
        InputManager.Instance.OnHotbarSelected += SelectSlot;
        SelectSlot(0);
    }

    void OnDestroy()
    {
        if (InputManager.Instance != null) InputManager.Instance.OnHotbarSelected -= SelectSlot;
    }

    public void PickupItem(ItemSettings item)
    {
        if (item == null)
        {
            return;
        }

        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i] == null)
            {
                slots[i] = item;
                
                if (i == currentSelectedIndex) 
                {
                    if (Bootstrapper.Inventory != null) Bootstrapper.Inventory.TakeObject(item);
                    else
                    {
                        Debug.LogError("HotbarManager: Bootstrapper.Inventory равен null! Предмет добавлен в слот, но не взят в руки.");
                    }
                }
                else  item.gameObject.SetActive(false);
                OnInventoryChanged?.Invoke();
                return;
            }
        }
        
        radialItems.Add(item);
        item.gameObject.SetActive(false);
        OnInventoryChanged?.Invoke();
    }

    public void MoveItem(ItemContainer fromContainer, int fromIndex, ItemContainer toContainer, int toIndex)
    {
        ItemSettings draggedItem = (fromContainer == ItemContainer.Hotbar) ? slots[fromIndex] : radialItems[fromIndex];
        if (draggedItem == null) return; 

        ItemSettings targetItem = null;
        if (toContainer == ItemContainer.Hotbar) targetItem = slots[toIndex];
        else if (toIndex < radialItems.Count) targetItem = radialItems[toIndex];

        if (fromContainer == ItemContainer.Hotbar) slots[fromIndex] = targetItem;
        else
        {
            if (targetItem == null) radialItems.RemoveAt(fromIndex);
            else radialItems[fromIndex] = targetItem;
        }

        if (toContainer == ItemContainer.Hotbar) slots[toIndex] = draggedItem;
        else
        {
            if (targetItem == null) radialItems.Add(draggedItem);
            else radialItems[toIndex] = draggedItem;
        }

        SelectSlot(currentSelectedIndex); 
        OnInventoryChanged?.Invoke();     
    }

    public void SelectSlot(int index)
    {
        if (index < 0 || index >= slotCount || Time.timeScale == 0) return;
        if (Bootstrapper.Inventory == null) 
        {
            return;
        }
        if (index == currentSelectedIndex && slots[index] == Bootstrapper.Inventory.CurrentItem) return; 

        currentSelectedIndex = index;
        Bootstrapper.Inventory.HideObjectInPocket();

        ItemSettings selectedItem = slots[currentSelectedIndex];
        if (selectedItem != null) Bootstrapper.Inventory.TakeObject(selectedItem);
        OnInventoryChanged?.Invoke();
    }

    public void RemoveCurrentItem()
    {
        slots[currentSelectedIndex] = null;
        OnInventoryChanged?.Invoke();
    }
}

public enum ItemContainer { Hotbar, Radial }