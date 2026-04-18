using UnityEngine;
using UnityEngine.Audio;
using System.IO;
using UnityEngine.UI;

[System.Serializable]
public class AudioSettingsData
{
    public float masterVolume = 0.5f;
    public float musicVolume = 0.5f;
    public float sfxVolume = 0.5f;
    public float uiVolume = 0.5f;
    public float voiceVolume = 0.5f;
    public float ambienceVolume = 0.5f;

    public void SetDefaults()
    {
        masterVolume = 0.5f;
        musicVolume = 0.5f;
        sfxVolume = 0.5f;
        uiVolume = 0.5f;
        voiceVolume = 0.5f;
        ambienceVolume = 0.5f;
    }
}

public class AudioController : MonoBehaviour
{
    [SerializeField] AudioMixer mainMixer;
    AudioSettingsData currentSettings;
    string savePath;

    void Awake()
    {
        savePath = Path.Combine(Application.persistentDataPath, "AudioSettings.json");
        LoadSettings();
    }

    void Start() => ApplySettingsToMixer();

    void LoadSettings()
    {
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            currentSettings = JsonUtility.FromJson<AudioSettingsData>(json);
        }
        else currentSettings = new AudioSettingsData();
    }

    public void SaveSettings()
    {
        string json = JsonUtility.ToJson(currentSettings, true); 
        File.WriteAllText(savePath, json);
    }

    public void ResetSettings()
    {
        currentSettings.SetDefaults();
        ApplySettingsToMixer();
    }

    void ApplySettingsToMixer()
    {
        SetVolume("MasterVolume", currentSettings.masterVolume);
        SetVolume("MusicVolume", currentSettings.musicVolume);
        SetVolume("SFXVolume", currentSettings.sfxVolume);
        SetVolume("UIVolume", currentSettings.uiVolume);
        SetVolume("VoiceVolume", currentSettings.voiceVolume);
        SetVolume("AmbienceVolume", currentSettings.ambienceVolume);
    }
    void SetVolume(string exposedParamName, float sliderValue)
    {
        float clampedValue = Mathf.Clamp(sliderValue, 0.0001f, 1f);
        float volumeInDb = Mathf.Log10(clampedValue) * 20f;
        mainMixer.SetFloat(exposedParamName, volumeInDb);
    }

    public void SetMasterVolume(float sliderValue)
    {
        currentSettings.masterVolume = sliderValue;
        SetVolume("MasterVolume", sliderValue);
    }

    public void SetMusicVolume(float sliderValue)
    {
        currentSettings.musicVolume = sliderValue;
        SetVolume("MusicVolume", sliderValue);
    }

    public void SetSFXVolume(float sliderValue)
    {
        currentSettings.sfxVolume = sliderValue;
        SetVolume("SFXVolume", sliderValue);
    }

    public void SetUIVolume(float sliderValue)
    {
        currentSettings.uiVolume = sliderValue;
        SetVolume("UIVolume", sliderValue);
    }

    public void SetVoiceVolume(float sliderValue)
    {
        currentSettings.voiceVolume = sliderValue;
        SetVolume("VoiceVolume", sliderValue);
    }

    public void SetAmbienceVolume(float sliderValue)
    {
        currentSettings.ambienceVolume = sliderValue;
        SetVolume("AmbienceVolume", sliderValue);
    }

    public AudioSettingsData GetCurrentSettings() => currentSettings;
}