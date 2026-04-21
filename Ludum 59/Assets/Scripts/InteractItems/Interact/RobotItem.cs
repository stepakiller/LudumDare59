using UnityEngine;

public class RobotItem : MonoBehaviour, Interactable
{
    [SerializeField] CollectItems myController;
    [SerializeField] AudioSource audioSource;
    public void Interact()
    {
        audioSource.Play();
        myController.ActivateNextElement();
        Destroy(gameObject);
    }
}
