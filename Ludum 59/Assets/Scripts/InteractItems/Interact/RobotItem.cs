using UnityEngine;

public class RobotItem : MonoBehaviour, Interactable
{
    [SerializeField] CollectItems myController;
    public void Interact()
    {
        myController.ActivateNextElement();
        Destroy(gameObject);
    }
}
