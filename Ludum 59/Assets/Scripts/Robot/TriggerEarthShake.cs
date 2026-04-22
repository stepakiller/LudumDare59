using UnityEngine;

public class TriggerEarthShake : MonoBehaviour
{
    [SerializeField] EarthquakeController earthquakeController;
    [SerializeField] LayerMask _hazardLayer;
    [SerializeField] float duration = 5f;
    [SerializeField] float force = 2f;
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip earthShake;
    void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & _hazardLayer) != 0)
        {
            EarthShake();
            Destroy(other.gameObject);
        }
    }

    void EarthShake()
    {
        earthquakeController.TriggerCustomEarthquake(duration,force);
        audioSource.PlayOneShot(earthShake);
    }
}