using UnityEngine;
using System.Collections;

public class RedAlertBlinker : MonoBehaviour
{
    [SerializeField] AudioSource warningSiren;
    [SerializeField] float _blinkInterval = 0.5f;
    [SerializeField] Color _normalColor = Color.yellow;
    [SerializeField] Color _alarmColor = Color.red;
    [SerializeField] Material _mainLampsMaterial;
    [SerializeField] Material _sirenMaterial;
    [SerializeField] Light[] _pointLights;

    static readonly int EmissionColorId = Shader.PropertyToID("_EmissionColor");

    Coroutine _blinkCoroutine;
    bool _isAlarmActive = false;
    WaitForSeconds _cachedWait;
    Color _originalMainColor;
    Color _originalSirenColor;

    int _activeBreakdownsCount = 0;

    void Awake()
    {
        _cachedWait = new WaitForSeconds(_blinkInterval);
        if (_mainLampsMaterial != null) _originalMainColor = _mainLampsMaterial.GetColor(EmissionColorId);
        if (_sirenMaterial != null) _originalSirenColor = _sirenMaterial.GetColor(EmissionColorId);
        SetNormalState();
    }

    void OnEnable()
    {
        BreakDown.OnAnyBreakdownOccurred += HandleBreakdownOccurred;
        BreakDown.OnAnyBreakdownFixed += HandleBreakdownFixed;
    }

    void OnDisable()
    {
        BreakDown.OnAnyBreakdownOccurred -= HandleBreakdownOccurred;
        BreakDown.OnAnyBreakdownFixed -= HandleBreakdownFixed;
        if (_isAlarmActive) StopAlarm();
    }

    void HandleBreakdownOccurred()
    {
        _activeBreakdownsCount++;
        if (_activeBreakdownsCount > 0 && !_isAlarmActive)
        {
            StartAlarm();
        }
    }

    void HandleBreakdownFixed()
    {
        _activeBreakdownsCount--;
        if (_activeBreakdownsCount <= 0)
        {
            _activeBreakdownsCount = 0; 
            StopAlarm();
        }
    }

    public void StartAlarm()
    {
        warningSiren.Play();
        if (_isAlarmActive) return;
        _isAlarmActive = true;
        _blinkCoroutine = StartCoroutine(BlinkRoutine());
    }

    public void StopAlarm()
    {
        warningSiren.Pause();
        if (!_isAlarmActive) return;
        _isAlarmActive = false;
        if (_blinkCoroutine != null) StopCoroutine(_blinkCoroutine);
        SetNormalState();
    }

    IEnumerator BlinkRoutine()
    {
        bool isLightOn = false;

        while (_isAlarmActive)
        {
            isLightOn = !isLightOn;
            Color currentEmission = isLightOn ? _alarmColor : Color.black;
            if (_mainLampsMaterial != null) _mainLampsMaterial.SetColor(EmissionColorId, currentEmission);
            if (_sirenMaterial != null) _sirenMaterial.SetColor(EmissionColorId, currentEmission);
            foreach (var light in _pointLights)
            {
                if (light != null)
                {
                    light.color = _alarmColor; 
                    light.gameObject.SetActive(isLightOn); 
                }
            }
            yield return _cachedWait;
        }
    }

    void SetNormalState()
    {
        if (_mainLampsMaterial != null) _mainLampsMaterial.SetColor(EmissionColorId, _normalColor);
        if (_sirenMaterial != null) _sirenMaterial.SetColor(EmissionColorId, Color.black);
        foreach (var light in _pointLights)
        {
            if (light != null)
            {
                light.color = _normalColor;
                light.gameObject.SetActive(true);
            }
        }
    }

    void OnDestroy()
    {
        if (_mainLampsMaterial != null) _mainLampsMaterial.SetColor(EmissionColorId, _originalMainColor);
        if (_sirenMaterial != null) _sirenMaterial.SetColor(EmissionColorId, _originalSirenColor);
    }
}