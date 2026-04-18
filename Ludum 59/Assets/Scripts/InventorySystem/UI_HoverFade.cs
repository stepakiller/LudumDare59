using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class UI_HoverFade : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Настройки прозрачности")]
    [SerializeField] float defaultAlpha = 0.5f;
    [SerializeField] float hoverAlpha = 1.0f;
    [SerializeField] float fadeDuration = 0.2f;

    [SerializeField] CanvasGroup canvasGroup;
    [SerializeField] UI_DraggableItem draggableItem;
    bool isHovered = false;

    void Start() =>  RefreshState();

    public void RefreshState()
    {
        if (draggableItem.ContainerType != ItemContainer.Hotbar) return;
        canvasGroup.DOKill();
        canvasGroup.alpha = isHovered ? hoverAlpha : defaultAlpha;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovered = true;
        if (draggableItem.ContainerType != ItemContainer.Hotbar) return;
        canvasGroup.DOKill();
        canvasGroup.DOFade(hoverAlpha, fadeDuration).SetUpdate(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovered = false;
        if (draggableItem.ContainerType != ItemContainer.Hotbar) return;
        canvasGroup.DOKill();
        canvasGroup.DOFade(defaultAlpha, fadeDuration).SetUpdate(true);
    }

    void OnDisable()
    {
        isHovered = false;
        if (draggableItem != null && canvasGroup != null) RefreshState();
    }
}