using UnityEngine;
using TMPro;
using System.Collections;
using DG.Tweening;

public class NotificationManager : MonoBehaviour
{
    public static NotificationManager Instance { get; private set; }

    [SerializeField] GameObject _notificationWindow;
    [SerializeField] TextMeshProUGUI _notificationText;
    [SerializeField] CanvasGroup _canvasGroup;
    [SerializeField] float _fadeDuration = 0.5f;
    [SerializeField] float _displayTime = 2f;
    [SerializeField] Ease _showEase = Ease.OutCubic;
    [SerializeField] Ease _hideEase = Ease.InCubic;

    Coroutine _hideCoroutine;
    Tween _currentFadeTween;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        if (_notificationWindow != null)
        {
            _notificationWindow.SetActive(false);
            if (_canvasGroup != null) _canvasGroup.alpha = 0f;
        }
    }

    public void ShowMessage(string message)
    {
        if (_notificationWindow == null || _canvasGroup == null) return;
        _notificationText.text = message;
        _currentFadeTween?.Kill(); 
        if (_hideCoroutine != null) StopCoroutine(_hideCoroutine);
        _notificationWindow.SetActive(true);
        _currentFadeTween = _canvasGroup.DOFade(1f, _fadeDuration)
            .SetEase(_showEase)
            .SetUpdate(true);
        _hideCoroutine = StartCoroutine(HideAfterDelay());
    }

    private IEnumerator HideAfterDelay()
    {
        yield return new WaitForSeconds(_displayTime);
        _currentFadeTween = _canvasGroup.DOFade(0f, _fadeDuration)
            .SetEase(_hideEase)
            .OnComplete(() => _notificationWindow.SetActive(false));
    }
}