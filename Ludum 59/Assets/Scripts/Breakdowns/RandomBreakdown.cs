using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RandomBreakdown : MonoBehaviour
{
    [SerializeField] BreakDown[] _breakdowns; 
    [SerializeField] float _timeBetweenEventsMin = 5f;
    [SerializeField] float _timeBetweenEventsMax = 15f;
    
    void Start() => StartCoroutine(EventRoutine());

    IEnumerator EventRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(_timeBetweenEventsMin, _timeBetweenEventsMax));
            TriggerNextEvent();
        }
    }

    void TriggerNextEvent()
    {
        if (_breakdowns == null || _breakdowns.Length == 0) return;
        List<int> availableIndices = new List<int>();
        
        for (int i = 0; i < _breakdowns.Length; i++)
        {
            if (_breakdowns[i] != null && !_breakdowns[i].enabled) availableIndices.Add(i);
        }
        if (availableIndices.Count == 0) return;
        int randomIndex = availableIndices[Random.Range(0, availableIndices.Count)];
        _breakdowns[randomIndex].enabled = true;
    }
}