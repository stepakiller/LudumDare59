using UnityEngine;
using DG.Tweening;

public class SettingsButton : MonoBehaviour
{
    [SerializeField] RectTransform canvasRect;
    [SerializeField] RectTransform pauseMenu;
    [SerializeField] RectTransform settingsMenu;
    [SerializeField] float animationDuration = 0.5f;
    [SerializeField] Ease animationEase = Ease.OutBack;

    Vector2 pauseMenuCenterPos;
    float pauseMenuHiddenPosX;
    float settingsMenuHiddenPosY;
    public bool settingsIsOpen { get; private set; } = false;
    void Start()
    {
        pauseMenuCenterPos = pauseMenu.anchoredPosition;
        pauseMenuHiddenPosX = -(canvasRect.rect.width / 2f) - (pauseMenu.rect.width / 2f);
        settingsMenuHiddenPosY = canvasRect.rect.height + (settingsMenu.rect.height / 2f);
        settingsMenu.anchoredPosition = new Vector2(0f, settingsMenuHiddenPosY);
    }

    public void OpenSettings()
    {
        settingsIsOpen = true;
        pauseMenu.DOAnchorPosX(pauseMenuHiddenPosX, animationDuration).SetEase(animationEase).SetUpdate(true);
        settingsMenu.DOAnchorPosY(0f, animationDuration).SetEase(animationEase).SetUpdate(true);
    }

    public void CloseSettings()
    {
        settingsIsOpen = false;
        pauseMenu.DOAnchorPos(pauseMenuCenterPos, animationDuration).SetEase(animationEase).SetUpdate(true);
        settingsMenu.DOAnchorPosY(settingsMenuHiddenPosY, animationDuration).SetEase(animationEase).SetUpdate(true);
    }
}