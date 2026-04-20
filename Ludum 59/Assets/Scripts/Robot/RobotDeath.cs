using UnityEngine;
public class RobotDeath : MonoBehaviour
{
    [SerializeField] GameObject _deathScreen;
    [SerializeField] LayerMask _hazardLayer;
    bool _isDead = false;

    void Start() => _deathScreen.SetActive(false);

    private void OnTriggerEnter(Collider other)
    {
        if (_isDead) return;
        if (((1 << other.gameObject.layer) & _hazardLayer) != 0) Die();
    }

    void Die()
    {
        _isDead = true;
        _deathScreen.SetActive(true);
        TriggerDeathEffects();
    }
    void TriggerDeathEffects()
    {
        // 
    }
}