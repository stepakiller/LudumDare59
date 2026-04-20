using UnityEngine;
using TMPro;

public sealed class DepthTracker : MonoBehaviour
{
    [SerializeField] Transform _playerTransform;
    [SerializeField] Transform _referencePoint;
    [SerializeField] TMP_Text _depthText;
    [SerializeField] int _startDepth = 4900;
    [SerializeField] int _updateStep = 10;

    int _lastDisplayedDepth = int.MinValue;

    void Update()
    {
        if (_playerTransform == null || _referencePoint == null || _depthText == null) return;
        float heightDelta = _referencePoint.position.y - _playerTransform.position.y;
        float totalDepthRaw = _startDepth + heightDelta;
        int steppedDepth = Mathf.FloorToInt(totalDepthRaw / _updateStep) * _updateStep;
        if (steppedDepth != _lastDisplayedDepth)
        {
            _depthText.SetText("Depth: {0}m", steppedDepth);
            _lastDisplayedDepth = steppedDepth;
        }
    }
}
