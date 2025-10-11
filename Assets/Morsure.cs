using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Morsure : MonoBehaviour
{
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnCollisionEnter(Collision collision)
    {
        // Vérifie si le collider a un composant SystemedeSante
        systeme_sante systemedesante = collision.gameObject.GetComponent<systeme_sante>();
        if (systemedesante != null)
        {
            systemedesante.TakeDamage(5f); // Fait subir 10 points de dégats lors de la collision
            Debug.Log("Morsure");
        }
    }
}
