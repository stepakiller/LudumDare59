using UnityEngine;
using UnityEngine.EventSystems;

public class UI_Slot : MonoBehaviour, IDropHandler
{
    [field: SerializeField] public int SlotIndex { get; set; }
    [field: SerializeField] public ItemContainer ContainerType { get; set; }
    
    public void OnDrop(PointerEventData eventData)
    {
        GameObject droppedObj = eventData.pointerDrag;
        if (droppedObj == null) return;

        UI_DraggableItem draggableItem = droppedObj.GetComponent<UI_DraggableItem>();
        if (draggableItem != null) 
        {
            Bootstrapper.HotbarManager.MoveItem(draggableItem.ContainerType, draggableItem.SlotIndex, this.ContainerType, this.SlotIndex);
        }
    }
}
