using UnityEngine;

public class ItemSettings : MonoBehaviour
{
    public ItemData ItemData;
    public Rigidbody Rb;
    public Collider Col;
    public Equippable EquippableComponent;
    public Vector3 HoldPositionOffset;
    public Vector3 HoldRotationOffset;
    public float HandScale = 1f;
}