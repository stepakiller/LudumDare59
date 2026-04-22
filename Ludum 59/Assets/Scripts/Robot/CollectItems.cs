using UnityEngine;
using System.Collections;

public sealed class CollectItems : MonoBehaviour
{
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip screamer1;
    [SerializeField] AudioClip screamer2;
    [SerializeField] AudioClip udar;
    [SerializeField] EarthquakeController earthquakeController;
    [SerializeField] GameObject[] mans;
    [SerializeField] LinearCompass _compass;
    [SerializeField] RepairController _repairController;
    [SerializeField] GameObject[] _uiImages;
    [SerializeField] Transform[] _worldTargets;

    int _currentIndex = 0;

    void Start()
    {
        if (_repairController != null) _repairController.enabled = false;
        foreach (var img in _uiImages)
        {
            if (img != null) img.SetActive(false);
        }
        UpdateCompassTarget();
    }

    public void ActivateNextElement()
    {
        if (_currentIndex < _uiImages.Length && _uiImages[_currentIndex] != null) _uiImages[_currentIndex].SetActive(true);
        _currentIndex++;
        UpdateCompassTarget();
        if(_currentIndex == 1) Invoke(nameof(PlaySound), 2f);
        else if (_currentIndex == 2) StartCoroutine(PlaySoundAndWaitRoutine(screamer2));
        else if (_currentIndex == 4) for (int i = 0; i < mans.Length; i++) mans[i].SetActive(true);
    }

    void UpdateCompassTarget()
    {
        if (_compass == null) return;
        if (_currentIndex < _worldTargets.Length) _compass.SetTarget(_worldTargets[_currentIndex]);
        else _compass.SetTarget(null);
    }

    void PlaySound() => audioSource.PlayOneShot(screamer1);

    IEnumerator PlaySoundAndWaitRoutine(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
        yield return new WaitForSeconds(clip.length);
        audioSource.PlayOneShot(udar);
        _repairController.enabled = true;
    }
}