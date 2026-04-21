using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.UI;

public class RepairController : MonoBehaviour
{
    [SerializeField] RepairableObject[] breakdowns;
    [SerializeField] float timeBetweenEventsMin = 10f;
    [SerializeField] float timeBetweenEventsMax = 30f;
    [SerializeField] TextMeshProUGUI oxygenText;
    [SerializeField] Image oxygenFillImage;
    [SerializeField] GameObject gameOverScreen;
    [SerializeField] float blinkSpeed = 0.5f;
    [SerializeField] int maxOxygen = 100;
    [SerializeField] int oxygenDrainPerTick = 5;
    [SerializeField] float oxygenDrainInterval = 2f;
    [SerializeField] int warningThreshold = 60;
    [SerializeField] int criticalThreshold = 30;

    int _currentOxygen;
    int _lastEventIndex = -1;
    int _activeBreakdownsCount = 0;
    
    Coroutine _oxygenDrainCoroutine;
    Coroutine _blinkCoroutine;
    WaitForSeconds _oxygenDrainWait;
    WaitForSeconds _blinkWait;

    void Start()
    {
        _oxygenDrainWait = new WaitForSeconds(oxygenDrainInterval);
        _blinkWait = new WaitForSeconds(blinkSpeed);
        _currentOxygen = maxOxygen;
        UpdateOxygenUI();
        foreach (var breakdown in breakdowns)
        {
            breakdown.OnBroken += HandleBreakdown;
            breakdown.OnRepaired += HandleRepair;
        }
        TriggerNextEvent();
        StartCoroutine(EventRoutine());
    }

    void OnDestroy()
    {
        foreach (var breakdown in breakdowns)
        {
            if (breakdown != null)
            {
                breakdown.OnBroken -= HandleBreakdown;
                breakdown.OnRepaired -= HandleRepair;
            }
        }
    }

    IEnumerator EventRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(timeBetweenEventsMin, timeBetweenEventsMax));
            TriggerNextEvent();
        }
    }

    void TriggerNextEvent()
    {
        if (breakdowns.Length == 0) return;
        bool hasIntactObjects = false;
        foreach (var b in breakdowns)
        {
            if (b.IsRepaired) hasIntactObjects = true;
        }
        if (!hasIntactObjects) return;
        int newIndex;
        int safetyCounter = 0;
        do
        {
            newIndex = Random.Range(0, breakdowns.Length);
            safetyCounter++;
        }
        while ((newIndex == _lastEventIndex || !breakdowns[newIndex].IsRepaired) && safetyCounter < 100);
        _lastEventIndex = newIndex;
        breakdowns[newIndex].Break();
    }

    void HandleBreakdown()
    {
        _activeBreakdownsCount++;
        if (_activeBreakdownsCount > 0 && _oxygenDrainCoroutine == null)
        {
            _oxygenDrainCoroutine = StartCoroutine(OxygenDrainRoutine());
            _blinkCoroutine = StartCoroutine(BlinkImageRoutine()); 
        }
    }

    void HandleRepair()
    {
        _activeBreakdownsCount--;
        if (_activeBreakdownsCount <= 0)
        {
            if (_oxygenDrainCoroutine != null) StopCoroutine(_oxygenDrainCoroutine);
            if (_blinkCoroutine != null) StopCoroutine(_blinkCoroutine);
            _oxygenDrainCoroutine = null;
            _blinkCoroutine = null;
            _activeBreakdownsCount = 0;
            UpdateOxygenUI();
        }
    }

    IEnumerator OxygenDrainRoutine()
    {
        while (_currentOxygen > 0)
        {
            yield return _oxygenDrainWait;
            _currentOxygen -= oxygenDrainPerTick;
            _currentOxygen = Mathf.Max(0, _currentOxygen);
            UpdateOxygenUI();
            if (_currentOxygen <= 0)
            {
                TriggerGameOver();
                yield break;
            }
        }
    }

    IEnumerator BlinkImageRoutine()
    {
        bool isRed = false;
        while (true)
        {
            if (oxygenFillImage != null) oxygenFillImage.color = isRed ? Color.red : new Color(0f, 1.000f, 0.000f, 1.000f);
            isRed = !isRed;
            yield return new WaitForSeconds(blinkSpeed);
        }
    }

    void UpdateOxygenUI()
    {
        if (oxygenText == null || oxygenFillImage == null) return;
        float fillPercentage = (float)_currentOxygen / maxOxygen;
        Color targetColor = new Color(0f, 1.000f, 0.000f, 1.000f);
        if (_currentOxygen <= criticalThreshold) targetColor = Color.red;
        else if (_currentOxygen <= warningThreshold) targetColor = Color.yellow;
        oxygenText.text = $"{_currentOxygen}%";
        oxygenText.color = targetColor; 
        oxygenFillImage.fillAmount = fillPercentage;
        if (_blinkCoroutine == null) oxygenFillImage.color = targetColor;
    }

    void TriggerGameOver()
    {
        if (gameOverScreen != null) gameOverScreen.SetActive(true);
        Time.timeScale = 0f;
    }
}