using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class systeme_sante : MonoBehaviour
{
    public event System.Action<float> OnChangedSante;

    [SerializeField] private float max_health;
    [SerializeField] private float current_health;

    public float maxhealth
    {
        get { return max_health; }
    }

    public float currenthealth
    {
        get { return current_health; }
    }

    public bool IsDead
    {
        get { return current_health <= 0; }
    }

    private void Start()
    {
        // Initialise avec la santé maximale quand le jeu est lancé
        current_health = max_health;

        // Informe les autres classes à propos de la santé initiale
        if (OnChangedSante != null)
        {
            OnChangedSante(ObtenirSanteNormalisee());
        }
    }

    public float ObtenirSanteNormalisee()
    {
        return current_health / max_health;
    }

    public void TakeDamage(float damage)
    {
        if (IsDead) return;

        current_health = Mathf.Clamp(current_health - damage, 0f, max_health);
        Debug.Log($"{gameObject.name} took {damage} damage. Health: {current_health}/{max_health}");

        // Notifier les "listeners" de la modification de la santé
        if (OnChangedSante != null)
        {
            OnChangedSante(ObtenirSanteNormalisee());
        }

        if (current_health <= 0)
        {
            // On peut gérer la mort ici ou faire une classe externe
            Debug.Log($"{gameObject.name} est mort!");
        }
    }

    public void Heal(float amount)
    {
        current_health = Mathf.Clamp(current_health + amount, 0f, max_health);
        Debug.Log($"{gameObject.name} healed for {amount}. Health: {current_health}/{max_health}");

        // Notifier les listeners de la modification de la santé
        if (OnChangedSante != null)
        {
            OnChangedSante(ObtenirSanteNormalisee());
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
