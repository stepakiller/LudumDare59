using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class ScannableEnemy : MonoBehaviour, IScannable
{
    [SerializeField] float _scanThreshold = 15f; 
    [SerializeField] float _scanDecayRate = 5f; 
    [SerializeField] AudioClip _revealSound;
    [SerializeField] AudioSource _audioSource;
    [SerializeField] Renderer[] _renderers;
    [SerializeField] Collider[] _colliders;

    float _currentScanAmount = 0f;
    bool _isRevealed = false;

    void Reset()
    {
        _audioSource = GetComponent<AudioSource>();
        _audioSource.playOnAwake = false;
        _audioSource.spatialBlend = 1f;

        // Автоматически собираем компоненты на самом объекте (и его детях, если нужно)
        _renderers = GetComponentsInChildren<Renderer>();
        _colliders = GetComponentsInChildren<Collider>();
    }

    void Update()
    {
        // Если враг уже обнаружен, шкалу больше не трогаем
        if (_isRevealed) return;

        if (_currentScanAmount > 0)
        {
            _currentScanAmount -= _scanDecayRate * Time.deltaTime;
            _currentScanAmount = Mathf.Max(0, _currentScanAmount); 
        }
    }

    public void OnScanned(Vector3 hitPoint)
    {
        // Если уже просканирован и исчезает — игнорируем новые лучи
        if (_isRevealed) return; 

        _currentScanAmount += 1f;

        if (_currentScanAmount >= _scanThreshold)
        {
            StartCoroutine(VanishRoutine());
        }
    }

    IEnumerator VanishRoutine()
    {
        _isRevealed = true; // Блокируем логику сканирования

        // 1. Отключаем визуал, чтобы враг "исчез"
        foreach (var rend in _renderers) 
        {
            if (rend != null) rend.enabled = false;
        }

        // 2. Отключаем коллайдеры, чтобы лучи пролетали насквозь
        foreach (var col in _colliders) 
        {
            if (col != null) col.enabled = false;
        }

        // 3. Издаем звук и ждем его окончания
        if (_revealSound != null && _audioSource != null)
        {
            _audioSource.PlayOneShot(_revealSound);
            
            // Ждем ровно столько секунд, сколько длится аудиоклип
            yield return new WaitForSeconds(_revealSound.length);
        }
        else
        {
            // Если звука нет, ждем хотя бы один кадр для безопасности
            yield return null; 
        }

        // 4. Полностью удаляем объект из сцены
        Destroy(gameObject);
    }
}