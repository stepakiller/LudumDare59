using UnityEngine;
using System.Collections;

public class RandomEvent : MonoBehaviour
{
    [SerializeField] Behaviour[] breakdowns;
    [SerializeField] float timeBetweenEvents = 10f;
    int _lastEventIndex = -1;

    void Start() => StartCoroutine(EventRoutine());

    IEnumerator EventRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(timeBetweenEvents);
            TriggerNextEvent();
        }
    }

    void TriggerNextEvent()
    {
        int newIndex;
        do newIndex = Random.Range(0, breakdowns.Length);
        while (newIndex == _lastEventIndex);
        if (breakdowns[newIndex] != null) breakdowns[newIndex].enabled = true;
        _lastEventIndex = newIndex;
    }
}
