using UnityEngine;
using TMPro; 

public class SignalProximityManager : MonoBehaviour
{
    [SerializeField] Transform player;
    [SerializeField] Transform targetObject;
    [SerializeField] TextMeshProUGUI signalText;
    [SerializeField] float victoryDistance = 2f;

    float _maxDistance; 
    bool _isInitialized = false;

    void Update()
    {
        if (targetObject == null) return;
        if (!_isInitialized)
        {
            _maxDistance = Vector3.Distance(player.position, targetObject.position);
            if (_maxDistance <= victoryDistance)  _maxDistance = victoryDistance + 1f;
            _isInitialized = true;
        }
        float currentDistance = Vector3.Distance(player.position, targetObject.position);
        float signalFactor = Mathf.InverseLerp(victoryDistance, _maxDistance, currentDistance);
        int signalPercentage = Mathf.RoundToInt(signalFactor * 100f);
        if (signalText != null) signalText.text = $"Signal: {signalPercentage}%";
    }
}
