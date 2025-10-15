using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class systeme_sante : MonoBehaviour
{
    public event System.Action<float> OnChangedSante;

    public event System.Action OnDeath;

    [SerializeField] private float max_health = 100f;
    [SerializeField] private float current_health;

    public float maxhealth => max_health;
    public float currenthealth => current_health;
    public bool IsDead => current_health <= 0;

    private bool hasDied = false;

    private void Start()
    {
        current_health = max_health;
        OnChangedSante?.Invoke(ObtenirSanteNormalisee());
    }

    public float ObtenirSanteNormalisee()
    {
        return current_health / max_health;
    }

    public void TakeDamage(float damage)
    {
        if (IsDead) return;

        current_health = Mathf.Clamp(current_health - damage, 0f, max_health);

        OnChangedSante?.Invoke(ObtenirSanteNormalisee());

        if (current_health <= 0 && !hasDied)
        {
            hasDied = true;
            Die();
        }
    }

    public void Heal(float amount)
    {
        if (IsDead) return;

        current_health = Mathf.Clamp(current_health + amount, 0f, max_health);
        OnChangedSante?.Invoke(ObtenirSanteNormalisee());
    }

    private void Die()
    {

        OnDeath?.Invoke();

        if (CompareTag("Player"))
        {
            Destroy(gameObject, 3f);
        }
        else
        {
            Pursuit pursuit = GetComponent<Pursuit>();
            if (pursuit != null)
                pursuit.enabled = false;

            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = false;
                rb.constraints = RigidbodyConstraints.None;
            }

            Destroy(gameObject, 3f);
        }
    }
}
