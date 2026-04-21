using UnityEngine;

public interface Interactable
{
    void Interact();
}

public interface IHoldInteractable : Interactable
{
    void CancelInteract();
}