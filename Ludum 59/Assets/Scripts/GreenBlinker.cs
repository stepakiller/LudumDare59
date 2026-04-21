using UnityEngine;
using System.Collections;

public class GreenBlinker : MonoBehaviour
{
    [Header("Настройки")]
    [Tooltip("Материал, который будет мигать")]
    [SerializeField] private Material _targetMaterial;
    
    [Tooltip("Базовый цвет свечения")]
    [SerializeField] private Color _emissionColor = Color.green;
    
    [Tooltip("Интенсивность свечения (HDR)")]
    [SerializeField] private float _emissionIntensity = 2f;
    
    [Tooltip("Скорость мигания в секундах")]
    [SerializeField] private float _blinkInterval = 0.5f;

    // Оптимизация
    private static readonly int EmissionColorId = Shader.PropertyToID("_EmissionColor");
    private WaitForSeconds _cachedWait;

    // Для защиты оригинального ассета
    private Color _originalColor;

    private void Start()
    {
        if (_targetMaterial == null)
        {
            Debug.LogWarning("Материал не назначен в скрипт SimpleGreenBlinker!", this);
            return;
        }

        _originalColor = _targetMaterial.GetColor(EmissionColorId);
        _cachedWait = new WaitForSeconds(_blinkInterval);

        StartCoroutine(BlinkRoutine());
    }

    private IEnumerator BlinkRoutine()
    {
        bool isGlowing = false;
        
        // Считаем итоговый HDR-цвет один раз до начала цикла
        // Умножение цвета на число увеличивает его яркость
        Color finalGlowingColor = _emissionColor * _emissionIntensity;

        while (true)
        {
            isGlowing = !isGlowing;

            // Выбираем: либо наш усиленный цвет, либо черный (выключено)
            Color currentColor = isGlowing ? finalGlowingColor : Color.black;
            _targetMaterial.SetColor(EmissionColorId, currentColor);

            yield return _cachedWait;
        }
    }

    private void OnDestroy()
    {
        if (_targetMaterial != null)
        {
            _targetMaterial.SetColor(EmissionColorId, _originalColor);
        }
    }
}