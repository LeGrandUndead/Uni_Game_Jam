using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet_Damage : MonoBehaviour
{
    public float damageAmount = 10f;

    private void OnCollisionEnter(Collision collision)
    {
        systeme_sante targetHealth = collision.gameObject.GetComponent<systeme_sante>();
        if (targetHealth != null)
        {
            targetHealth.TakeDamage(damageAmount);

            if (targetHealth.IsDead)
            {
                Destroy(collision.gameObject); // Removes the enemy
            }
        }

        Destroy(gameObject); // Destroy bullet on hit
    }
}
