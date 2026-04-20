using UnityEngine;
using System.Collections;
using TMPro;

public class RandomBreakdown : MonoBehaviour
{

    [SerializeField] Behaviour[] breakdowns;
    [SerializeField] float timeBetweenEventsMin;
    [SerializeField] float timeBetweenEventsMax;
    int _lastEventIndex = -1;

    void Start() => StartCoroutine(EventRoutine());

    IEnumerator EventRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(timeBetweenEventsMin, timeBetweenEventsMax));
            TriggerNextEvent();
        }
    }

    void TriggerNextEvent()
    {
        if (breakdowns == null || breakdowns.Length == 0) return;
        if (breakdowns.Length == 1)
        {
            if (breakdowns[0] != null) breakdowns[0].enabled = true;
            return;
        }
        int newIndex;
        int safetyCounter = 0;
        do 
        {
            newIndex = Random.Range(0, breakdowns.Length);
            safetyCounter++;
        }
        while (newIndex == _lastEventIndex && safetyCounter < 100);
        if (breakdowns[newIndex] != null) breakdowns[newIndex].enabled = true;
        _lastEventIndex = newIndex;
    }
}
