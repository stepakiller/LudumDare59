using UnityEngine;
using Unity.Cinemachine;

public class EarthquakeController : MonoBehaviour
{
    float _duration = 5f;
    float _force = 2f;
    CinemachineImpulseSource _impulseSource;
    void Awake() => _impulseSource = GetComponent<CinemachineImpulseSource>();
    public void TriggerEarthquake() => TriggerCustomEarthquake(_duration, _force);
    public void TriggerCustomEarthquake(float customDuration, float customForce)
    {
        _impulseSource.ImpulseDefinition.TimeEnvelope.SustainTime = customDuration;
        _impulseSource.GenerateImpulseWithForce(customForce);
        Debug.Log("Землятресение");
    }
}