using System.Collections;
using UnityEngine;

public class ScannableEnemy : MonoBehaviour, IScannable
{
    [SerializeField] float _scanThreshold = 15f; 
    [SerializeField] float _scanDecayRate = 5f; 
    [SerializeField] AudioClip _revealSound;
    [SerializeField] AudioSource _audioSource;
    [SerializeField] Renderer[] _renderers;
    [SerializeField] Collider[] _colliders;
    [SerializeField] FinalEvent finalEvent;
    float _currentScanAmount = 0f;
    bool _isRevealed = false;

    void Reset()
    {
        _audioSource = GetComponent<AudioSource>();
        _audioSource.playOnAwake = false;
        _audioSource.spatialBlend = 1f;
        _renderers = GetComponentsInChildren<Renderer>();
        _colliders = GetComponentsInChildren<Collider>();
    }

    void Update()
    {
        if (_isRevealed) return;
        if (_currentScanAmount > 0)
        {
            _currentScanAmount -= _scanDecayRate * Time.deltaTime;
            _currentScanAmount = Mathf.Max(0, _currentScanAmount); 
        }
    }

    public void OnScanned(Vector3 hitPoint)
    {
        if (_isRevealed) return; 
        _currentScanAmount += 1f;
        if (_currentScanAmount >= _scanThreshold)
        {
            finalEvent.AddCount();
            StartCoroutine(VanishRoutine());
        }
    }

    IEnumerator VanishRoutine()
    {
        _isRevealed = true;
        foreach (var rend in _renderers) 
        {
            if (rend != null) rend.enabled = false;
        }
        foreach (var col in _colliders) 
        {
            if (col != null) col.enabled = false;
        }
        if (_revealSound != null && _audioSource != null)
        {
            _audioSource.PlayOneShot(_revealSound);
            yield return new WaitForSeconds(_revealSound.length);
        }
        else yield return null;
        Destroy(gameObject);
    }
}