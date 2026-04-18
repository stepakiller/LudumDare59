using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class ItemData : ScriptableObject
{
    [field: SerializeField] public string ItemName { get; private set; }
    [field: SerializeField, TextArea(3, 10)] public string ItemDescription { get; private set; }
    [field: SerializeField] public Sprite Icon { get; private set; }
    [field: SerializeField] public GameObject Prefab { get; private set; }
    [field: SerializeField] public float InspectScaleMultiplier { get; private set; } = 1f;
}
