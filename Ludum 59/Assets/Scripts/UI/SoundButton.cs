using UnityEngine;

public class SoundButton : MonoBehaviour
{
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip soundButton;
    public void PlaySound() => audioSource.PlayOneShot(soundButton);
}
