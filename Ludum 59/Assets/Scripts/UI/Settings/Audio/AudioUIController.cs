using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Globalization;
public class AudioUIController : MonoBehaviour
{
    [SerializeField] AudioController audioController;
    [SerializeField] Slider masterSlider;
    [SerializeField] Slider musicSlider;
    [SerializeField] Slider sfxSlider;
    [SerializeField] Slider uiSlider;
    [SerializeField] Slider voiceSlider;
    [SerializeField] Slider ambienceSlider;

    [SerializeField] TMP_InputField masterInput;
    [SerializeField] TMP_InputField musicInput;
    [SerializeField] TMP_InputField sfxInput;
    [SerializeField] TMP_InputField uiInput;
    [SerializeField] TMP_InputField voiceInput;
    [SerializeField] TMP_InputField ambienceInput;

    [SerializeField] Button applyButton;
    [SerializeField] Button resetButton;

    void OnEnable()
    {
        InitializeUI();

        if (applyButton != null) applyButton.onClick.AddListener(OnApplyClicked);
        if (resetButton != null) resetButton.onClick.AddListener(OnResetClicked);

        if (masterSlider != null) masterSlider.onValueChanged.AddListener(OnMasterSliderChanged);
        if (musicSlider != null) musicSlider.onValueChanged.AddListener(OnMusicSliderChanged);
        if (sfxSlider != null) sfxSlider.onValueChanged.AddListener(OnSFXSliderChanged);
        if (uiSlider != null) uiSlider.onValueChanged.AddListener(OnUISliderChanged);
        if (voiceSlider != null) voiceSlider.onValueChanged.AddListener(OnVoiceSliderChanged);
        if (ambienceSlider != null) ambienceSlider.onValueChanged.AddListener(OnAmbienceSliderChanged);

        if (masterInput != null) masterInput.onEndEdit.AddListener(OnMasterInputChanged);
        if (musicInput != null) musicInput.onEndEdit.AddListener(OnMusicInputChanged);
        if (sfxInput != null) sfxInput.onEndEdit.AddListener(OnSFXInputChanged);
        if (uiInput != null) uiInput.onEndEdit.AddListener(OnUIInputChanged);
        if (voiceInput != null) voiceInput.onEndEdit.AddListener(OnVoiceInputChanged);
        if (ambienceInput != null) ambienceInput.onEndEdit.AddListener(OnAmbienceInputChanged);
    }

    void OnDisable()
    {
        if (applyButton != null) applyButton.onClick.RemoveListener(OnApplyClicked);
        if (resetButton != null) resetButton.onClick.RemoveListener(OnResetClicked);
        
        if (masterSlider != null) masterSlider.onValueChanged.RemoveAllListeners();
        if (musicSlider != null) musicSlider.onValueChanged.RemoveAllListeners();
        if (sfxSlider != null) sfxSlider.onValueChanged.RemoveAllListeners();
        if (uiSlider != null) uiSlider.onValueChanged.RemoveAllListeners();
        if (voiceSlider != null) voiceSlider.onValueChanged.RemoveAllListeners();
        if (ambienceSlider != null) ambienceSlider.onValueChanged.RemoveAllListeners();

        if (masterInput != null) masterInput.onEndEdit.RemoveAllListeners();
        if (musicInput != null) musicInput.onEndEdit.RemoveAllListeners();
        if (sfxInput != null) sfxInput.onEndEdit.RemoveAllListeners();
        if (uiInput != null) uiInput.onEndEdit.RemoveAllListeners();
        if (voiceInput != null) voiceInput.onEndEdit.RemoveAllListeners();
        if (ambienceInput != null) ambienceInput.onEndEdit.RemoveAllListeners();
    }

    void InitializeUI()
    {
        if (audioController == null) return;
        AudioSettingsData data = audioController.GetCurrentSettings();
        if (data == null) return;

        SetSliderAndInputWithoutNotify(masterSlider, masterInput, data.masterVolume);
        SetSliderAndInputWithoutNotify(musicSlider, musicInput, data.musicVolume);
        SetSliderAndInputWithoutNotify(sfxSlider, sfxInput, data.sfxVolume);
        SetSliderAndInputWithoutNotify(uiSlider, uiInput, data.uiVolume);
        SetSliderAndInputWithoutNotify(voiceSlider, voiceInput, data.voiceVolume);
        SetSliderAndInputWithoutNotify(ambienceSlider, ambienceInput, data.ambienceVolume);
    }

    string FormatValueForUI(float value)
    {
        return (value * 100f).ToString("F0", CultureInfo.InvariantCulture);
    }

    void SetSliderAndInputWithoutNotify(Slider slider, TMP_InputField input, float value)
    {
        slider.SetValueWithoutNotify(value);
        if (input != null) input.SetTextWithoutNotify(FormatValueForUI(value));
    }

    float ParseSafe(string textValue)
    {
        textValue = textValue.Replace(',', '.');
        if (float.TryParse(textValue, NumberStyles.Float, CultureInfo.InvariantCulture, out float result))
        {
            float normalizedValue = result / 100f;
            return Mathf.Clamp(normalizedValue, 0.0001f, 1f);
        }
        return -1f;
    }

    void OnMasterSliderChanged(float val)
    {
        audioController.SetMasterVolume(val);
        if (masterInput != null) masterInput.SetTextWithoutNotify(FormatValueForUI(val));
    }
    void OnMasterInputChanged(string textVal)
    {
        float val = ParseSafe(textVal);
        if (val >= 0) { if (masterSlider != null) masterSlider.value = val; }
        else { if (masterInput != null) masterInput.SetTextWithoutNotify(FormatValueForUI(masterSlider.value)); }
    }

    void OnMusicSliderChanged(float val)
    {
        audioController.SetMusicVolume(val);
        if (musicInput != null) musicInput.SetTextWithoutNotify(FormatValueForUI(val));
    }
    void OnMusicInputChanged(string textVal)
    {
        float val = ParseSafe(textVal);
        if (val >= 0) { if (musicSlider != null) musicSlider.value = val; }
        else { if (musicInput != null) musicInput.SetTextWithoutNotify(FormatValueForUI(musicSlider.value)); }
    }

    void OnSFXSliderChanged(float val)
    {
        audioController.SetSFXVolume(val);
        if (sfxInput != null) sfxInput.SetTextWithoutNotify(FormatValueForUI(val));
    }
    void OnSFXInputChanged(string textVal)
    {
        float val = ParseSafe(textVal);
        if (val >= 0) { if (sfxSlider != null) sfxSlider.value = val; }
        else { if (sfxInput != null) sfxInput.SetTextWithoutNotify(FormatValueForUI(sfxSlider.value)); }
    }

    void OnUISliderChanged(float val)
    {
        audioController.SetUIVolume(val);
        if (uiInput != null) uiInput.SetTextWithoutNotify(FormatValueForUI(val));
    }
    void OnUIInputChanged(string textVal)
    {
        float val = ParseSafe(textVal);
        if (val >= 0) { if (uiSlider != null) uiSlider.value = val; }
        else { if (uiInput != null) uiInput.SetTextWithoutNotify(FormatValueForUI(uiSlider.value)); }
    }

    void OnVoiceSliderChanged(float val)
    {
        audioController.SetVoiceVolume(val);
        if (voiceInput != null) voiceInput.SetTextWithoutNotify(FormatValueForUI(val));
    }
    void OnVoiceInputChanged(string textVal)
    {
        float val = ParseSafe(textVal);
        if (val >= 0) { if (voiceSlider != null) voiceSlider.value = val; }
        else { if (voiceInput != null) voiceInput.SetTextWithoutNotify(FormatValueForUI(voiceSlider.value)); }
    }

    void OnAmbienceSliderChanged(float val)
    {
        audioController.SetAmbienceVolume(val);
        if (ambienceInput != null) ambienceInput.SetTextWithoutNotify(FormatValueForUI(val));
    }
    void OnAmbienceInputChanged(string textVal)
    {
        float val = ParseSafe(textVal);
        if (val >= 0) { if (ambienceSlider != null) ambienceSlider.value = val; }
        else { if (ambienceInput != null) ambienceInput.SetTextWithoutNotify(FormatValueForUI(ambienceSlider.value)); }
    }

    void OnApplyClicked() => audioController.SaveSettings();
    void OnResetClicked()
    {
        audioController.ResetSettings();
        InitializeUI(); 
    }
}