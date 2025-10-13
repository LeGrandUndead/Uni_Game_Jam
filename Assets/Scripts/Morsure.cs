using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Morsure : MonoBehaviour
{
    public float damage = 5f;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            return; 
        }

        systeme_sante systemedesante = collision.gameObject.GetComponent<systeme_sante>();
        if (systemedesante != null)
        {
            systemedesante.TakeDamage(damage);
            Debug.Log($"{gameObject.name} attacked {collision.gameObject.name} for {damage} damage!");
        }
    }
}
