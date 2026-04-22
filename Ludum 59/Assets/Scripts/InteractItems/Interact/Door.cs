using UnityEngine;
using DG.Tweening;

public class Door : MonoBehaviour, Interactable
{
    [SerializeField] float _openAngle = 90f;
    [SerializeField] float _duration = 0.8f;
    [SerializeField] Ease _moveEase = Ease.OutQuad;
    [SerializeField] AudioSource _audioSource;
    [SerializeField] AudioClip _openSound;
    [SerializeField] AudioClip _closeSound;
    bool _isOpen = false;
    Vector3 _defaultRotation;
    Transform _doorTransform;

    void Start()
    {
        _doorTransform = transform;
        _defaultRotation = _doorTransform.localEulerAngles;
    }

    public void Interact()
    {
        if (DOTween.IsTweening(_doorTransform)) return;
        if (!_isOpen) OpenDoor();
        else CloseDoor();
    }

    void OpenDoor()
    {
        _audioSource.PlayOneShot(_openSound);
        Transform player = Bootstrapper.PlayerTransform;
        if (player == null)
        {
            return; 
        }
        Vector3 directionToPlayer = player.position - _doorTransform.position;
        directionToPlayer.y = 0; 
        float dot = Vector3.Dot(_doorTransform.forward, directionToPlayer.normalized);
        float targetAngleY = (dot < 0) ? -_openAngle : _openAngle;
        Vector3 targetRotation = new Vector3(_defaultRotation.x, _defaultRotation.y + targetAngleY, _defaultRotation.z);
        _doorTransform.DOLocalRotate(targetRotation, _duration).SetEase(_moveEase);
        _isOpen = true;
    }

    void CloseDoor()
    {
        _audioSource.PlayOneShot(_closeSound);
        _doorTransform.DOLocalRotate(_defaultRotation, _duration).SetEase(_moveEase);
        _isOpen = false;
    }
}

