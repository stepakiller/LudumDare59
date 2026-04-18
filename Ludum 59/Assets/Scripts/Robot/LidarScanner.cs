using UnityEngine;
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
    [SerializeField] Transform scannerOrigin;
    [SerializeField] ParticleSystem pointCloudSystem;
    [SerializeField] float scanRange = 20f;
    [SerializeField] float horizontalFOV = 60f;
    [SerializeField] float verticalFOV = 40f;
    [SerializeField] int raysPerFrame = 100;
    [SerializeField] LayerMask scanLayer;
    [SerializeField] float sweepDuration = 1.5f;
    [SerializeField] float cooldownDuration = 1f;
    [SerializeField] Color defaultPointColor = Color.white;
    [SerializeField] float pointSize = 0.05f;
    [SerializeField] float lineThickness = 2f;
    
    [SerializeField] List<ScanColorMapping> colorMappings; 

    Coroutine _scanCoroutine;
    float _lastScanTime = -Mathf.Infinity;

    void Start()
    {
        if (InputManager.Instance != null) InputManager.Instance.OnLidarScannerPressed += TryToggleScan;
    }

    void OnDestroy()
    {
        if (InputManager.Instance != null) InputManager.Instance.OnLidarScannerPressed -= TryToggleScan;
    }

    void TryToggleScan()
    {
        if (Time.time < _lastScanTime + cooldownDuration) 
        {
            //добавить звук неудачного использования сканера
            return;
        }
        _lastScanTime = Time.time;
        if (_scanCoroutine != null) StopCoroutine(_scanCoroutine);        
        _scanCoroutine = StartCoroutine(ScanSweepCoroutine());
    }

    IEnumerator ScanSweepCoroutine()
    {
        float timer = 0f;
        while (timer < sweepDuration)
        {
            timer += Time.deltaTime;
            float progress = timer / sweepDuration;
            float currentVerticalAngle = Mathf.Lerp(verticalFOV, -verticalFOV, progress);
            ScanSweepFrame(currentVerticalAngle);
            yield return null; 
        }
    }

    void ScanSweepFrame(float baseVerticalAngle)
    {
        for (int i = 0; i < raysPerFrame; i++)
        {
            float randomYaw = Random.Range(-horizontalFOV, horizontalFOV);
            float randomPitch = baseVerticalAngle + Random.Range(-lineThickness, lineThickness);
            Quaternion randomRotation = Quaternion.Euler(randomPitch, randomYaw, 0f);
            Vector3 rayDirection = scannerOrigin.rotation * randomRotation * Vector3.forward;
            if (Physics.Raycast(scannerOrigin.position, rayDirection, out RaycastHit hit, scanRange, scanLayer)) 
            {
                Color finalColor = GetColorForHit(hit.collider);
                EmitPoint(hit.point, hit.normal, finalColor);
            }
        }
    }

    Color GetColorForHit(Collider hitCollider)
    {
        foreach (var mapping in colorMappings)
        {
            if (hitCollider.CompareTag(mapping.tag))
            {
                return mapping.color;
            }
        }
        return defaultPointColor;
    }

    void EmitPoint(Vector3 position, Vector3 normal, Color color)
    {
        ParticleSystem.EmitParams emitParams = new ParticleSystem.EmitParams
        {
            position = position,
            startColor = color,
            startSize = pointSize
        };
        pointCloudSystem.Emit(emitParams, 1);
    }
}