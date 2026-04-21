using UnityEngine;
using UnityEngine.UI;

public class RayCast : MonoBehaviour
{
    [Header("Crosshair Settings")]
    [SerializeField] private Image _crosshairImage; 
    [SerializeField] private float _defaultAlpha = 0.5f;
    [SerializeField] private float _activeAlpha = 1f;
    [SerializeField] private Vector3 _defaultScale = Vector3.one; 
    [SerializeField] private Vector3 _activeScale = new Vector3(1.2f, 1.2f, 1.2f); 
    [SerializeField] private float _crosshairAnimationSpeed = 15f; 

    [Header("Raycast Settings")]
    [SerializeField] private Camera _camera;
    [SerializeField] private float _rayDistance = 3f;
    [SerializeField] private LayerMask _interactLayer;
    
    // Теперь используем IInteractable
    public Interactable CurrentInteractable => _currentInteractable;
    
    private bool _isLookingAtInteractable = false;
    private Interactable _currentInteractable; 

    private float _targetAlpha;
    private Vector3 _targetScale;

    private void Start()
    {
        _targetAlpha = _defaultAlpha;
        _targetScale = _defaultScale;
        
        if (_crosshairImage != null)
        {
            Color c = _crosshairImage.color;
            c.a = _targetAlpha;
            _crosshairImage.color = c;
            _crosshairImage.rectTransform.localScale = _targetScale;
        }
        
        InputManager.Instance.OnInteractPressed += TryInteract;
    }

    private void OnDestroy()
    {
        if (InputManager.Instance != null) InputManager.Instance.OnInteractPressed -= TryInteract;
    }

    private void Update()
    {
        Ray ray = new Ray(_camera.transform.position, _camera.transform.forward);
        
        if (Physics.Raycast(ray, out RaycastHit hit, _rayDistance, _interactLayer) && hit.collider.TryGetComponent(out Interactable interactable))
        {
            if (_currentInteractable != interactable)
            {
                // ПРОВЕРКА: Если прошлый объект поддерживал отмену, отменяем его
                if (_currentInteractable is IHoldInteractable holdablePrevious) 
                {
                    holdablePrevious.CancelInteract();
                }

                _currentInteractable = interactable;
                SetCrosshairTargets(_activeAlpha, _activeScale);
                _isLookingAtInteractable = true;
            }
        }
        else
        {
            if (_isLookingAtInteractable)
            {
                // ПРОВЕРКА: Если мы отвернулись, и объект поддерживает отмену
                if (_currentInteractable is IHoldInteractable holdableCurrent) 
                {
                    holdableCurrent.CancelInteract();
                }

                _currentInteractable = null;
                SetCrosshairTargets(_defaultAlpha, _defaultScale);
                _isLookingAtInteractable = false;
            }
        }
        
        AnimateCrosshair();

        // Отпускание кнопки (замени на нужную)
        if (Input.GetKeyUp(KeyCode.E))
        {
            CancelInteract();
        }
    }

    private void TryInteract()
    {
        if (_currentInteractable != null) _currentInteractable.Interact();
    }

    private void CancelInteract()
    {
        // ПРОВЕРКА при отпускании кнопки
        if (_currentInteractable is IHoldInteractable holdable) 
        {
            holdable.CancelInteract();
        }
    }

    private void SetCrosshairTargets(float alpha, Vector3 targetScale)
    {
        _targetAlpha = alpha;
        _targetScale = targetScale;
    }

    private void AnimateCrosshair()
    {
        if (_crosshairImage == null) return;
        _crosshairImage.rectTransform.localScale = Vector3.Lerp(_crosshairImage.rectTransform.localScale, _targetScale, Time.deltaTime * _crosshairAnimationSpeed);
        Color currentColor = _crosshairImage.color;
        currentColor.a = Mathf.Lerp(currentColor.a, _targetAlpha, Time.deltaTime * _crosshairAnimationSpeed);
        _crosshairImage.color = currentColor;
    }
}