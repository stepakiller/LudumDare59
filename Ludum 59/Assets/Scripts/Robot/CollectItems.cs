using UnityEngine;

public sealed class CollectItems : MonoBehaviour
{
    [SerializeField] LinearCompass _compass;
    [SerializeField] RepairController _repairController;
    [SerializeField] GameObject[] _uiImages;
    [SerializeField] Transform[] _worldTargets;

    int _currentIndex = 0;
    bool _isRepairEnabled = false;

    void Start()
    {
        if (_repairController != null) _repairController.enabled = false;
        foreach (var img in _uiImages)
        {
            if (img != null) img.SetActive(false);
        }
        UpdateCompassTarget();
    }

    public void ActivateNextElement()
    {
        if (!_isRepairEnabled && _currentIndex == 1)
        {
            _repairController.enabled = true;
            _isRepairEnabled = true;
        }

        if (_currentIndex < _uiImages.Length && _uiImages[_currentIndex] != null) _uiImages[_currentIndex].SetActive(true);
        _currentIndex++;
        UpdateCompassTarget();
    }

    void UpdateCompassTarget()
    {
        if (_compass == null) return;
        if (_currentIndex < _worldTargets.Length) _compass.SetTarget(_worldTargets[_currentIndex]);
        else _compass.SetTarget(null);
    }
}