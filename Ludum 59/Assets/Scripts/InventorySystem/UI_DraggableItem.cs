using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Image), typeof(CanvasGroup))]
public class UI_DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [field: SerializeField] public int SlotIndex { get; set; }
    [field: SerializeField] public ItemContainer ContainerType { get; set; }
    Transform originalParent;
    Image image;
    CanvasGroup canvasGroup;
    Canvas rootCanvas; 
    RectTransform rectTransform;
    void Awake() => InitComponents();

    void InitComponents()
    {
        if (image == null) image = GetComponent<Image>();
        if (canvasGroup == null) canvasGroup = GetComponent<CanvasGroup>();
        if (rectTransform == null) rectTransform = GetComponent<RectTransform>();
        if (rootCanvas == null)
        {
            Canvas parentCanvas = GetComponentInParent<Canvas>();
            if (parentCanvas != null) rootCanvas = parentCanvas.rootCanvas;
        }
    }

    public void Setup(ItemData item, int index)
    {
        InitComponents(); 
        SlotIndex = index;
        if (item != null)
        {
            image.sprite = item.Icon;
            canvasGroup.alpha = 1f;
            canvasGroup.blocksRaycasts = true;
        }
        else
        {
            image.sprite = null;
            canvasGroup.alpha = 0f;
            canvasGroup.blocksRaycasts = false;
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalParent = transform.parent;
        if (rootCanvas != null)
        {
            transform.SetParent(rootCanvas.transform, true); 
            transform.SetAsLastSibling();
        }
        canvasGroup.blocksRaycasts = false; 
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (rootCanvas != null && RectTransformUtility.ScreenPointToLocalPointInRectangle(
            (RectTransform)rootCanvas.transform, 
            eventData.position, 
            eventData.pressEventCamera, 
            out Vector2 localPoint))
        {transform.position = rootCanvas.transform.TransformPoint(localPoint);}
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        transform.SetParent(originalParent, false); 
        rectTransform.localPosition = Vector3.zero; 
        rectTransform.anchoredPosition = Vector2.zero; 
        transform.localScale = Vector3.one;
        canvasGroup.blocksRaycasts = true;
    }
}