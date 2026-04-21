using UnityEngine;
using System;

public class BreakDown : MonoBehaviour
{
    [SerializeField] ParticleSystem _particles;
    [SerializeField] GameObject _pointOnMap;
    public static event Action OnAnyBreakdownOccurred;
    public static event Action OnAnyBreakdownFixed;

    void OnEnable()
    {
        if (_particles != null)
        {
            _particles.gameObject.SetActive(true);
            _particles.Play();
        }
        if (_pointOnMap != null)  _pointOnMap.SetActive(true);
        OnAnyBreakdownOccurred?.Invoke();
    }

    void OnDisable()
    {
        if (_particles != null)
        {
            _particles.gameObject.SetActive(false);
            _particles.Stop();
        }
        if (_pointOnMap != null)   _pointOnMap.SetActive(false);
        OnAnyBreakdownFixed?.Invoke();
    }
}