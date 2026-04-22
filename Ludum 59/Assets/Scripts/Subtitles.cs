using UnityEngine;
using DG.Tweening;
public class Subtitles : MonoBehaviour
{
    [Header("Настройки прозрачности")]
    [SerializeField] private float fadeDuration = 1f;
    [SerializeField] private Ease fadeEase = Ease.InOutSine;

    [Header("Настройки движения")]
    [SerializeField] private float waitDuration = 2f;
    
    [Tooltip("Скорость движения вверх. Для обычного UI ставь 300-800. Для объектов в мире (World Space) ставь 1-5.")]
    [SerializeField] private float moveSpeed = 5f;

    private CanvasGroup canvasGroup;
    private Vector3 startPosition;
    private Sequence animationSequence;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        
        // Жестко глушим альфу еще до включения объекта, чтобы избежать "моргания"
        canvasGroup.alpha = 0f; 
        
        startPosition = transform.position;
    }

    private void OnEnable()
    {
        StartAnimation();
    }

    private void OnDisable()
    {
        animationSequence?.Kill();
    }

    public void StartAnimation()
    {
        animationSequence?.Kill();
        
        // Сброс
        canvasGroup.alpha = 0f;
        transform.position = startPosition;

        animationSequence = DOTween.Sequence();

        // Шаг 1: Появление
        animationSequence.Append(canvasGroup.DOFade(1f, fadeDuration).SetEase(fadeEase));

        // Шаг 2: Пауза
        animationSequence.AppendInterval(waitDuration);

        // Шаг 3: Движение через обычный Transform (надежнее для объектов на сцене)
        // Двигаем вверх на 1 единицу со скоростью moveSpeed
        transform.DOLocalMoveY(1f, moveSpeed)
    .SetSpeedBased()
    .SetRelative()
    .SetLoops(-1, LoopType.Incremental)
    .SetEase(Ease.Linear);
    }
}