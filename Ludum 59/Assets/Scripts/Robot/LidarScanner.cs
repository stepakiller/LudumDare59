using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public struct ScanColorMapping
{
    public string tag;
    public Color color;
}

public class LidarScanner : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Transform _scannerOrigin;
    [SerializeField] ParticleSystem _pointCloudSystem;
    [SerializeField] Image _cooldownImage;

    [Header("Scan Settings")]
    [SerializeField] float _scanRange = 30f;
    [SerializeField] float _horizontalFOV = 60f;
    [SerializeField] float _verticalFOV = 40f;
    [SerializeField] int _raysPerFrame = 200;
    [SerializeField] LayerMask _scanLayer;
    [SerializeField] float _sweepDuration = 2;
    [SerializeField] float _cooldownDuration = 2f;
    
    [Header("Visual Settings")]
    [SerializeField] float _pointSize = 0.03f;
    [SerializeField] float _lineThickness = 2f;
    [SerializeField] Color _defaultPointColor = Color.white;
    [SerializeField] ScanColorMapping[] _colorMappings;

    Dictionary<string, Color> _tagToColorMap;
    Coroutine _scanCoroutine;
    float _lastScanTime = -Mathf.Infinity;
    ParticleSystem.EmitParams _emitParams;
    bool _isCoolingDown = false;
    HashSet<Collider> _hitCollidersThisFrame = new HashSet<Collider>();

    void Awake()
    {
        InitializeColorMap();
        _emitParams = new ParticleSystem.EmitParams{startSize = _pointSize};
    }

    void Start()
    {
        if (InputManager.Instance != null) InputManager.Instance.OnLidarScannerPressed += TryToggleScan;
    }

    void OnDestroy()
    {
        if (InputManager.Instance != null)  InputManager.Instance.OnLidarScannerPressed -= TryToggleScan;
    }

    void Update()
    {
        if (_isCoolingDown) UpdateCooldownUI();
    }

    void InitializeColorMap()
    {
        _tagToColorMap = new Dictionary<string, Color>();
        foreach (var mapping in _colorMappings)
        {
            if (!_tagToColorMap.ContainsKey(mapping.tag)) _tagToColorMap.Add(mapping.tag, mapping.color);
        }
    }

    void TryToggleScan()
    {
        if (Time.time < _lastScanTime + _cooldownDuration) 
        {
            return;
        }
        _lastScanTime = Time.time;
        _isCoolingDown = true;
        if (_scanCoroutine != null) StopCoroutine(_scanCoroutine);
        _scanCoroutine = StartCoroutine(ScanSweepCoroutine());
    }

    void UpdateCooldownUI()
    {
        if (_cooldownImage == null) return;
        float timeSinceLastScan = Time.time - _lastScanTime;
        float progress = Mathf.Clamp01(timeSinceLastScan / _cooldownDuration);
        _cooldownImage.fillAmount = progress;
        if (progress >= 1f) _isCoolingDown = false;
    }

    IEnumerator ScanSweepCoroutine()
    {
        float timer = 0f;
        while (timer < _sweepDuration)
        {
            timer += Time.deltaTime;
            float progress = timer / _sweepDuration;
            float currentVerticalAngle = Mathf.Lerp(_verticalFOV, -_verticalFOV, progress);
            ScanSweepFrame(currentVerticalAngle);
            yield return null; 
        }
    }

    void ScanSweepFrame(float baseVerticalAngle)
    {
        Vector3 originPos = _scannerOrigin.position;
        Quaternion originRot = _scannerOrigin.rotation;
        _hitCollidersThisFrame.Clear();
        for (int i = 0; i < _raysPerFrame; i++)
        {
            float randomYaw = Random.Range(-_horizontalFOV, _horizontalFOV);
            float randomPitch = baseVerticalAngle + Random.Range(-_lineThickness, _lineThickness);
            Quaternion randomRotation = Quaternion.Euler(randomPitch, randomYaw, 0f);
            Vector3 rayDirection = originRot * randomRotation * Vector3.forward;
            
            if (Physics.Raycast(originPos, rayDirection, out RaycastHit hit, _scanRange, _scanLayer)) 
            {
                Color finalColor = GetColorForHit(hit.collider);
                EmitPoint(hit.point, finalColor);
                if (_hitCollidersThisFrame.Add(hit.collider))
                {
                    if (hit.collider.TryGetComponent(out IScannable scannableObject)) scannableObject.OnScanned(hit.point);
                }
            }
        }
    }

    Color GetColorForHit(Collider hitCollider)
    {
        if (_tagToColorMap.TryGetValue(hitCollider.tag, out Color color))
        {
            return color;
        }
        return _defaultPointColor;
    }

    void EmitPoint(Vector3 position, Color color)
    {
        _emitParams.position = position;
        _emitParams.startColor = color;
        _pointCloudSystem.Emit(_emitParams, 1);
    }
}