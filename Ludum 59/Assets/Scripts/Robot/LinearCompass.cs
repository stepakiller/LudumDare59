using UnityEngine;

public class LinearCompass : MonoBehaviour
{
    [SerializeField] Transform _playerTransform;
    [SerializeField] RectTransform _compassBar;
    [SerializeField] RectTransform _targetIcon;
    [SerializeField] bool _clampToEdges = true;
    Transform _targetTransform; 
    float _halfCompassWidth;

    void Start()
    {
        _halfCompassWidth = _compassBar.rect.width / 2f;
    }

    void Update()
    {
        if (_playerTransform == null || _targetTransform == null) return;
        UpdateMarkerPosition();
    }
    public void SetTarget(Transform newTarget)
    {
        Debug.Log($"Компас получил новую цель: {(newTarget == null ? "ПУСТО (NULL)" : newTarget.name)}");
        _targetTransform = newTarget;
        _targetIcon.gameObject.SetActive(_targetTransform != null);
    }

    private void UpdateMarkerPosition()
    {
        Vector3 directionToTarget = _targetTransform.position - _playerTransform.position;
        directionToTarget.y = 0f;
        if (directionToTarget.sqrMagnitude < 0.001f) return;
        Vector3 playerForward = _playerTransform.forward;
        playerForward.y = 0f;
        float angle = Vector3.SignedAngle(playerForward, directionToTarget, Vector3.up);
        float normalizedAngle = angle / 180f;
        float targetPositionX = normalizedAngle * _halfCompassWidth;
        if (_clampToEdges)targetPositionX = Mathf.Clamp(targetPositionX, -_halfCompassWidth, _halfCompassWidth);
        _targetIcon.anchoredPosition = new Vector2(targetPositionX, _targetIcon.anchoredPosition.y);
    }
}