using UnityEngine;

public class Inventory : MonoBehaviour
{
    public ItemSettings CurrentItem { get; private set; } 

    [Header("Настройки рук")]
    [SerializeField] Transform holdPivot;
    [SerializeField] float smoothSpeed = 15f;
    
    [Header("Настройки выброса")]
    [SerializeField] Transform dropPoint;
    [SerializeField] float dropForce = 5f;

    Vector3 originalScale;

    void Awake() => Bootstrapper.Inventory = this; 

    void Start() => InputManager.Instance.OnDropPressed += HandleDropInput;
    void OnDestroy()
    {
        if (InputManager.Instance != null) InputManager.Instance.OnDropPressed -= HandleDropInput;
    }

    void Update()
    {
        if (CurrentItem != null)
        {
            Transform itemTransform = CurrentItem.transform;
            Vector3 targetPos = holdPivot.TransformPoint(CurrentItem.HoldPositionOffset);
            Quaternion targetRot = holdPivot.rotation * Quaternion.Euler(CurrentItem.HoldRotationOffset);

            itemTransform.position = Vector3.Lerp(itemTransform.position, targetPos, Time.deltaTime * smoothSpeed);
            itemTransform.rotation = Quaternion.Slerp(itemTransform.rotation, targetRot, Time.deltaTime * smoothSpeed);
            itemTransform.localScale = Vector3.Lerp(itemTransform.localScale, Vector3.one * CurrentItem.HandScale, Time.deltaTime * smoothSpeed);
        }
    }

    void HandleDropInput()
    {
        if (CurrentItem != null)
        {
            DropObject();
            Bootstrapper.HotbarManager.RemoveCurrentItem();
        }
    }

    public void TakeObject(ItemSettings item)
    {
        CurrentItem = item;
        CurrentItem.gameObject.SetActive(true);
        if (CurrentItem.EquippableComponent != null) CurrentItem.EquippableComponent.Equip();
        
        originalScale = CurrentItem.transform.localScale;
        if (!CurrentItem.Rb.isKinematic)
        {
            CurrentItem.Rb.linearVelocity = Vector3.zero;
            CurrentItem.Rb.angularVelocity = Vector3.zero;
        }
        CurrentItem.Rb.isKinematic = true;
        CurrentItem.Rb.useGravity = false;
        
        CurrentItem.Col.enabled = false;
    }

    public void HideObjectInPocket()
    {
        if (CurrentItem == null) return;
        if (CurrentItem.EquippableComponent != null) CurrentItem.EquippableComponent.Unequip();
        CurrentItem.gameObject.SetActive(false);
        CurrentItem = null;
    }

    public void DropObject()
    {
        if (CurrentItem == null) return;
        if (CurrentItem.EquippableComponent != null) CurrentItem.EquippableComponent.Unequip();

        Transform itemTransform = CurrentItem.transform;
        itemTransform.localScale = originalScale;
        itemTransform.position = dropPoint.position;
        itemTransform.rotation = Quaternion.identity;

        CurrentItem.Rb.isKinematic = false;
        CurrentItem.Rb.useGravity = true;
        CurrentItem.Rb.linearVelocity = Vector3.zero;
        CurrentItem.Rb.angularVelocity = Vector3.zero;
        CurrentItem.Col.enabled = true;

        CurrentItem.Rb.AddForce(dropPoint.forward * dropForce, ForceMode.Impulse);
        CurrentItem = null;
    }
}