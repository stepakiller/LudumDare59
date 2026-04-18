using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Globalization;

public class SaveBinds : MonoBehaviour
{
    [SerializeField] Button applyButton;
    [SerializeField] Button resetButton;
    [SerializeField] Slider mouseSensitivitySlider;
    [SerializeField] TMP_InputField mouseSensitivityInput;
    [SerializeField] float minMouseSens = 0.1f;
    [SerializeField] float maxMouseSens = 10f;

    void Start()
    {
        if (applyButton != null) applyButton.onClick.AddListener(SaveSettings);
        if (resetButton != null) resetButton.onClick.AddListener(ResetSettings);

        if (mouseSensitivitySlider != null)
        {
            mouseSensitivitySlider.minValue = minMouseSens;
            mouseSensitivitySlider.maxValue = maxMouseSens;
            mouseSensitivitySlider.value = InputManager.Instance.MouseSensitivity;
            mouseSensitivitySlider.onValueChanged.AddListener(OnMouseSliderChanged);
        }

        if (mouseSensitivityInput != null)
        {
            mouseSensitivityInput.text = InputManager.Instance.MouseSensitivity.ToString("F2");
            mouseSensitivityInput.onEndEdit.AddListener(OnMouseInputChanged); 
        }
    }

    void OnMouseSliderChanged(float value)
    {
        InputManager.Instance.MouseSensitivity = value;
        if (mouseSensitivityInput != null) mouseSensitivityInput.SetTextWithoutNotify(value.ToString("F2"));
    }

    void OnMouseInputChanged(string input)
    {
        if (float.TryParse(input.Replace(',', '.'), NumberStyles.Float, CultureInfo.InvariantCulture, out float parsedValue))
        {
            parsedValue = Mathf.Clamp(parsedValue, minMouseSens, maxMouseSens);
            
            InputManager.Instance.MouseSensitivity = parsedValue;
            if (mouseSensitivitySlider != null) mouseSensitivitySlider.value = parsedValue;
            mouseSensitivityInput.text = parsedValue.ToString("F2");
        }
        else mouseSensitivityInput.text = InputManager.Instance.MouseSensitivity.ToString("F2");
    }

    void SaveSettings() => InputManager.Instance.SaveBindings();

    void ResetSettings()
    {
        InputManager.Instance.ResetBindings();
        RebindButton.RefreshAllUI();
        
        if (mouseSensitivitySlider != null) mouseSensitivitySlider.value = InputManager.Instance.MouseSensitivity;
        else if (mouseSensitivityInput != null) mouseSensitivityInput.text = InputManager.Instance.MouseSensitivity.ToString("F2");
    }

    void OnDestroy()
    {
        if (applyButton != null) applyButton.onClick.RemoveListener(SaveSettings);
        if (resetButton != null) resetButton.onClick.RemoveListener(ResetSettings);
        if (mouseSensitivitySlider != null) mouseSensitivitySlider.onValueChanged.RemoveListener(OnMouseSliderChanged);
        if (mouseSensitivityInput != null) mouseSensitivityInput.onEndEdit.RemoveListener(OnMouseInputChanged);
    }
}