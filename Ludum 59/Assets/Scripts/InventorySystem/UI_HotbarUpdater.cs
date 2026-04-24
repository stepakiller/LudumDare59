using UnityEngine;
using UnityEngine.UI;

public class UI_HotbarUpdater : MonoBehaviour
{
    [SerializeField] UI_Slot[] _uiSlots;
    [SerializeField] UI_DraggableItem[] _uiItems;
    [SerializeField] Color _hasItemColor = Color.white;
    [SerializeField] Color _emptyItemColor = Color.black;

    void Start()
    {
        for (int i = 0; i < _uiSlots.Length; i++)
        {
            _uiSlots[i].SlotIndex = i;
            _uiItems[i].SlotIndex = i;
            _uiSlots[i].ContainerType = ItemContainer.Hotbar;
            _uiItems[i].ContainerType = ItemContainer.Hotbar;
        }
        Bootstrapper.HotbarManager.OnInventoryChanged += UpdateUI;
        UpdateUI(); 
    }

    void OnDestroy()
    {
        if (Bootstrapper.HotbarManager != null) Bootstrapper.HotbarManager.OnInventoryChanged -= UpdateUI;
    }

    void UpdateUI()
    {
        ItemSettings[] items = Bootstrapper.HotbarManager.slots; 
        
        for (int i = 0; i < items.Length; i++)
        {
            bool hasItem = items[i] != null;

            if (hasItem)  _uiItems[i].Setup(items[i].ItemData, i);
            else  _uiItems[i].Setup(null, i);
            if (_uiItems[i].TryGetComponent(out Image itemImage)) itemImage.color = hasItem ? _hasItemColor : _emptyItemColor;
            if (_uiItems[i].TryGetComponent(out UI_HoverFade hoverFade)) hoverFade.RefreshState();
        }
    }
}