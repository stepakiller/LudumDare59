using UnityEngine;

public class FinalEvent : MonoBehaviour
{
    [SerializeField] GameObject music;
    [SerializeField] GameObject subtitles;
    [SerializeField] RandomPunches randomPunches;
    [SerializeField] EarthquakeController earthquakeController;
    [SerializeField] TerminalController terminalController;
    [SerializeField] Collider terminalControllerColl;
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip hardSound;
    [SerializeField] GameObject blackScreen;
    [SerializeField] GameObject[] disableObj;
    [SerializeField] Behaviour[] disableScripts;
    [SerializeField] float whenPlayHardSound;
    [SerializeField] float whenOnBlackScreen;
    [SerializeField] float whenOnMusic;
    int count;
    
    public void AddCount()
    {
        count++;
        if(count >= 2)
        {
            Invoke(nameof(LeaveFromTerminal), 2f);
            terminalControllerColl.enabled = false;
            randomPunches.StartSequence();
            earthquakeController.TriggerCustomEarthquake(20,0.75f);
            Invoke(nameof(PlayHardSound), whenPlayHardSound);
            Invoke(nameof(OnBlackScreen), whenOnBlackScreen);
            Invoke(nameof(OnMusic), whenOnMusic);
        }
    }

    void LeaveFromTerminal() => terminalController.ExitRobotMode();
    void PlayHardSound() => audioSource.PlayOneShot(hardSound);
    void OnMusic()
    {
        subtitles.SetActive(true);
        music.SetActive(true);
    }
    void OnBlackScreen()
    {
        blackScreen.SetActive(true);
        for (int i = 0; i < disableObj.Length; i++)disableObj[i].SetActive(false);
        for (int i = 0; i < disableScripts.Length; i++)disableScripts[i].enabled = false;
    }
}
