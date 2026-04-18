using UnityEngine;
using System.Collections;

public class LidarScanner : MonoBehaviour
{
    [SerializeField] Transform scannerOrigin;
    [SerializeField] ParticleSystem pointCloudSystem;
    [SerializeField] float scanRange = 20f;
    [SerializeField] float horizontalFOV = 60f;
    [SerializeField] float verticalFOV = 40f;
    [SerializeField] int raysPerFrame = 100;
    [SerializeField] LayerMask scanLayer;
    [SerializeField] Color pointColor = Color.cyan;
    [SerializeField] float pointSize = 0.05f;
    [SerializeField] float lineThickness = 2f;
    [SerializeField] float sweepDuration = 1.5f;

    Coroutine _scanCoroutine;

    void Start()
    {
        if (InputManager.Instance != null) InputManager.Instance.OnLidarScannerPressed += ToggleScan;
    }

    void OnDestroy()
    {
        if (InputManager.Instance != null) InputManager.Instance.OnLidarScannerPressed -= ToggleScan;
    }

    void ToggleScan()
    {
        if (_scanCoroutine != null)  StopCoroutine(_scanCoroutine);        
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
            if (Physics.Raycast(scannerOrigin.position, rayDirection, out RaycastHit hit, scanRange, scanLayer)) EmitPoint(hit.point, hit.normal);
        }
    }

    void EmitPoint(Vector3 position, Vector3 normal)
    {
        ParticleSystem.EmitParams emitParams = new ParticleSystem.EmitParams
        {
            position = position,
            startColor = pointColor,
            startSize = pointSize
        };
        pointCloudSystem.Emit(emitParams, 1);
    }
}