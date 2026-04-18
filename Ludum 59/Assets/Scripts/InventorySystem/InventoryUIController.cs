using UnityEngine;
using DG.Tweening;

public class InventoryUIController : MonoBehaviour
{
    [SerializeField] GameObject inventoryCanvas;
    [SerializeField] GameObject crosshair;
    [SerializeField] CanvasGroup crosshairCanvasGroup;
    [SerializeField] float crosshairFadeDuration = 0.5f;
    
    bool isInventoryOpen = false;

    void Start()
    {
        inventoryCanvas.SetActive(false);
        InputManager.Instance.OnInventoryPressed += ToggleInventory;
    }

    void OnDestroy()
    {
        if (InputManager.Instance != null) InputManager.Instance.OnInventoryPressed -= ToggleInventory;
    }

    void ToggleInventory()
    {
        isInventoryOpen = !isInventoryOpen;
        inventoryCanvas.SetActive(isInventoryOpen);
        if (crosshairCanvasGroup != null) DOTween.Kill(crosshairCanvasGroup);
        if (isInventoryOpen)
        {
            if (crosshair != null) crosshair.SetActive(false);
            if (crosshairCanvasGroup != null) crosshairCanvasGroup.alpha = 0f;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            InputManager.Instance.EnableUIInput();
        }
        else
        {
            if (crosshair != null) crosshair.SetActive(true);
            if (crosshairCanvasGroup != null)
            {
                crosshairCanvasGroup.alpha = 0f;
                crosshairCanvasGroup.DOFade(1f, crosshairFadeDuration).SetUpdate(true);
            }
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            InputManager.Instance.EnablePlayerInput();
        }
    }
}