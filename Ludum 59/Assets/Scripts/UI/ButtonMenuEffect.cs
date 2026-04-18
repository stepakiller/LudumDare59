using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using System.Collections;

public class ButtonMenuEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] TMP_Text buttonText;

    [SerializeField] Color normalColor = new Color(0.5f, 0.5f, 0.5f, 1f);
    [SerializeField] Color hoverColor = Color.white;
    [SerializeField] float animDuration = 0.2f;

    Coroutine currentCoroutine;

    void Start() => buttonText.color = normalColor;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (currentCoroutine != null) StopCoroutine(currentCoroutine);
        currentCoroutine = StartCoroutine(AnimateButton(1f, hoverColor));
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (currentCoroutine != null) StopCoroutine(currentCoroutine);
        currentCoroutine = StartCoroutine(AnimateButton(0f, normalColor));
    }

    private IEnumerator AnimateButton(float targetFill, Color targetColor)
    {
        Color startColor = buttonText.color;
        float elapsed = 0f;

        while (elapsed < animDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float percent = elapsed / animDuration;
            buttonText.color = Color.Lerp(startColor, targetColor, percent);
            yield return null;
        }
        buttonText.color = targetColor;
    }
}
