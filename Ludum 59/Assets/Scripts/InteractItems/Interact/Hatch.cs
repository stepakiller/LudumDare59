using UnityEngine;
using DG.Tweening;

public class Hatch : MonoBehaviour, Interactable
{
    [SerializeField] float _openAngle = 90f;
    [SerializeField] float _duration = 0.8f;
    [SerializeField] Ease _moveEase = Ease.OutQuad;

    bool _isOpen = false;
    Vector3 _defaultRotation;
    Transform _hatchTransform;

    void Start()
    {
        _hatchTransform = transform;
        _defaultRotation = _hatchTransform.localEulerAngles;
    }

    public void Interact()
    {
        if (DOTween.IsTweening(_hatchTransform)) return;
        if (!_isOpen) OpenHatch();
        else CloseHatch();
    }

    private void OpenHatch()
    {
        Transform player = Bootstrapper.PlayerTransform;

        if (player == null)
        {
            return; 
        }

        Vector3 directionToPlayer = player.position - _hatchTransform.position;
        float dot = Vector3.Dot(_hatchTransform.forward, directionToPlayer.normalized);
        float targetAngleX = (dot < 0) ? -_openAngle : _openAngle;
        Vector3 targetRotation = new Vector3(_defaultRotation.x + targetAngleX, _defaultRotation.y, _defaultRotation.z);
        _hatchTransform.DOLocalRotate(targetRotation, _duration).SetEase(_moveEase);
        _isOpen = true;
    }

    void CloseHatch()
    {
        _hatchTransform.DOLocalRotate(_defaultRotation, _duration).SetEase(_moveEase);
        _isOpen = false;
    }
}
