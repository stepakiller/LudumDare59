using UnityEngine;
using UnityEngine.UI;
using System;

public class RepairableObject : MonoBehaviour, IHoldInteractable
{
    public event Action OnRepaired;
    public event Action OnBroken;

    [Header("Настройки починки")]
    [SerializeField] Behaviour _breakdownScript;
    [SerializeField] float _repairDuration = 2f;
    [SerializeField] ItemData _requiredItem;

    [Header("UI")]
    [SerializeField] Image _progressFillImage;
    [SerializeField] GameObject _uiCanvas;

    [Header("Аудио")]
    [SerializeField] AudioSource _audioSource;

    public bool IsRepaired { get; private set; } = true; 
    
    float _currentRepairProgress = 0f;
    bool _isRepairing = false;

    void Start()
    {
        if (_progressFillImage != null) _progressFillImage.fillAmount = 0f;
        if (_uiCanvas != null) _uiCanvas.SetActive(false);
    }

    public void Interact()
    {
        if (IsRepaired || _isRepairing) return;
        ItemData heldItem = Bootstrapper.Inventory?.CurrentItem?.ItemData;
        if (_requiredItem != null && heldItem != _requiredItem)
        {
            string itemName = _requiredItem.ItemName;
            NotificationManager.Instance.ShowMessage($"Needed: {itemName}!");
            return;
        }
        _isRepairing = true;
        if (_uiCanvas != null) _uiCanvas.SetActive(true);
        AudioClip itemSound = heldItem != null ? heldItem.UseSound : null;
        if (_audioSource != null && itemSound != null && !_audioSource.isPlaying)
        {
            _audioSource.clip = itemSound;
            _audioSource.loop = true; 
            _audioSource.Play();
        }
    }

    public void CancelInteract()
    {
        _isRepairing = false;
        _currentRepairProgress = 0f;
        if (_progressFillImage != null) _progressFillImage.fillAmount = 0f;
        if (_uiCanvas != null) _uiCanvas.SetActive(false);
        StopRepairSound();
    }

    void Update()
    {
        if (!_isRepairing || IsRepaired) return;
        _currentRepairProgress += Time.deltaTime;
        if (_progressFillImage != null) _progressFillImage.fillAmount = _currentRepairProgress / _repairDuration;
        if (_currentRepairProgress >= _repairDuration)
        {
            RepairComplete();
            CancelInteract(); 
        }
    }

    public void Break()
    {
        if (!IsRepaired) return;
        IsRepaired = false;
        if (_breakdownScript != null) _breakdownScript.enabled = true;
        OnBroken?.Invoke();
    }

    public void RepairComplete()
    {
        IsRepaired = true;
        if (_breakdownScript != null) _breakdownScript.enabled = false;
        StopRepairSound(); 
        OnRepaired?.Invoke();
    }

    void StopRepairSound()
    {
        if (_audioSource != null && _audioSource.isPlaying) _audioSource.Stop();
    }
}