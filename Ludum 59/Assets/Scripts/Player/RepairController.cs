using UnityEngine;
using UnityEngine.UI;

public class RepairController : MonoBehaviour
{
    [SerializeField] RayCast rayCast;
    [SerializeField] GameObject repairUIContainer; 
    [SerializeField] Image repairProgressBar; 

    RepairableObject _currentRepairTarget;
    float _currentRepairTime = 0f;

    void Start()
    {
        if (repairUIContainer != null) repairUIContainer.SetActive(false);
        if (repairProgressBar != null) repairProgressBar.fillAmount = 0f;
    }

    void Update()
    {
        if (rayCast != null && rayCast.CurrentInteractable is RepairableObject repairable && !repairable.isRepaired)
        {
            _currentRepairTarget = repairable;
            if (HasRequiredItem(repairable))
            {
                if (InputManager.Instance != null && InputManager.Instance.IsInteractHeld) ProcessRepair();
                else ResetRepair();
            }
            else ResetRepair();
        }
        else
        {
            _currentRepairTarget = null;
            ResetRepair();
        }
    }

    bool HasRequiredItem(RepairableObject target)
    {
        if (target.RequiredItem == null) return true;
        if (Bootstrapper.Inventory == null || Bootstrapper.Inventory.CurrentItem == null) return false;
        return Bootstrapper.Inventory.CurrentItem.ItemData == target.RequiredItem;
    }

    void ProcessRepair()
    {
        if (repairUIContainer != null && !repairUIContainer.activeSelf) repairUIContainer.SetActive(true);
        _currentRepairTime += Time.deltaTime;
        float progress = _currentRepairTime / _currentRepairTarget.RepairDuration;
        if (repairProgressBar != null) repairProgressBar.fillAmount = progress;
        if (_currentRepairTime >= _currentRepairTarget.RepairDuration)
        {
            _currentRepairTarget.RepairComplete();
            ResetRepair(); 
        }
    }

    void ResetRepair()
    {
        _currentRepairTime = 0f;
        if (repairProgressBar != null) repairProgressBar.fillAmount = 0f;
        if (repairUIContainer != null && repairUIContainer.activeSelf) repairUIContainer.SetActive(false);
    }
}