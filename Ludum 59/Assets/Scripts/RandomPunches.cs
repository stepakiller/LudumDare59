using UnityEngine;

public class RandomPunches : MonoBehaviour
{
    [SerializeField] private AudioSource[] audioSources;
    [SerializeField] float delayBetweenSounds = 1f;
    float delayTimer = 0f;
    int currentSourceIndex = 0;
    bool isSessionActive = false;
    bool isWaitingForNext = false; 

    void Update()
    {
        if (!isSessionActive) return;
        if (isWaitingForNext)
        {
            delayTimer += Time.deltaTime;
            if (delayTimer >= delayBetweenSounds)
            {
                isWaitingForNext = false; 
                PlayNextSource();         
            }
            return;
        }
        if (!audioSources[currentSourceIndex].isPlaying)
        {
            isWaitingForNext = true;
            delayTimer = 0f; 
        }
    }

    public void StartSequence()
    {
        if (audioSources == null || audioSources.Length == 0)
        {
            return;
        }
        delayTimer = 0f;
        currentSourceIndex = 0;
        isSessionActive = true;
        isWaitingForNext = false; 

        StopAllSources();
        audioSources[currentSourceIndex].Play();
    }
    public void StopSequence()
    {
        isSessionActive = false;
        isWaitingForNext = false;
        StopAllSources();
    }

    void PlayNextSource()
    {
        currentSourceIndex = (currentSourceIndex + 1) % audioSources.Length;
        audioSources[currentSourceIndex].Stop();
        audioSources[currentSourceIndex].Play();
    }

    void StopAllSources()
    {
        foreach (var source in audioSources)
        {
            if (source != null) source.Stop();
        }
    }
}