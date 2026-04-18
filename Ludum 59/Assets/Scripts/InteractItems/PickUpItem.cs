using UnityEngine;

public class PickUpItem : MonoBehaviour, Interactable
{
    private ItemSettings _settings;

    void Awake() => _settings = GetComponent<ItemSettings>();

    public void Interact()
    {
        if (_settings != null) Bootstrapper.HotbarManager.PickupItem(_settings);
        else Debug.LogWarning($"На объекте {name} нет ItemSettings, подбор невозможен!");
    }
}
