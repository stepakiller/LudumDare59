using UnityEngine;
using UnityEngine.UI;

public class RayCast : MonoBehaviour
{
    [Header("Crosshair Settings")]
    [SerializeField] Image crosshairImage; 
    [SerializeField] float defaultAlpha = 0.5f;
    [SerializeField] float activeAlpha = 1f;
    [SerializeField] Vector3 defaultScale = Vector3.one; 
    [SerializeField] Vector3 activeScale = new Vector3(1.2f, 1.2f, 1.2f); 
    
    [SerializeField] float crosshairAnimationSpeed = 15f; 

    [Header("Raycast Settings")]
    [SerializeField] Camera _camera;
    [SerializeField] float rayDistance = 3f;
    [SerializeField] LayerMask interactLayer;
    public Interactable CurrentInteractable => _currentInteractable;
    bool _isLookingAtInteractable = false;
    Interactable _currentInteractable; 

    float _targetAlpha;
    Vector3 _targetScale;

    void Start()
    {
        _targetAlpha = defaultAlpha;
        _targetScale = defaultScale;
        
        if (crosshairImage != null)
        {
            Color c = crosshairImage.color;
            c.a = _targetAlpha;
            crosshairImage.color = c;
            crosshairImage.rectTransform.localScale = _targetScale;
        }
        InputManager.Instance.OnInteractPressed += TryInteract;
    }

    void OnDestroy()
    {
        if (InputManager.Instance != null) InputManager.Instance.OnInteractPressed -= TryInteract;
    }

    void Update()
    {
        Ray ray = new Ray(_camera.transform.position, _camera.transform.forward);
        
        if (Physics.Raycast(ray, out RaycastHit hit, rayDistance, interactLayer) && hit.collider.TryGetComponent(out Interactable interactable))
        {
            _currentInteractable = interactable;
            if (!_isLookingAtInteractable)
            {
                SetCrosshairTargets(activeAlpha, activeScale);
                _isLookingAtInteractable = true;
            }
        }
        else
        {
            _currentInteractable = null;
            if (_isLookingAtInteractable)
            {
                SetCrosshairTargets(defaultAlpha, defaultScale);
                _isLookingAtInteractable = false;
            }
        }
        
        AnimateCrosshair();
    }

    void TryInteract()
    {
        if (_currentInteractable != null)  _currentInteractable.Interact();
    }

    void SetCrosshairTargets(float alpha, Vector3 targetScale)
    {
        _targetAlpha = alpha;
        _targetScale = targetScale;
    }

    void AnimateCrosshair()
    {
        if (crosshairImage == null) return;
        crosshairImage.rectTransform.localScale = Vector3.Lerp(crosshairImage.rectTransform.localScale, _targetScale, Time.deltaTime * crosshairAnimationSpeed);
        Color currentColor = crosshairImage.color;
        currentColor.a = Mathf.Lerp(currentColor.a, _targetAlpha, Time.deltaTime * crosshairAnimationSpeed);
        crosshairImage.color = currentColor;
    }
}