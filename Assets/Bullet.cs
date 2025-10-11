using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("Bullet Settings")]
    public float speed = 40f;
    public float lifeTime = 5f;
    public float damage = 10f;
    public float knockbackForce = 5f;

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        // Move straight forward every frame
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    void OnTriggerEnter(Collider other)
    {
        // Ignore the player
        if (other.CompareTag("Player")) return;

        // If it hits an enemy
        if (other.CompareTag("Enemy"))
        {
            // --- Deal Damage ---
            systeme_sante health = other.GetComponent<systeme_sante>();
            if (health != null)
            {
                health.TakeDamage(damage);
            }

            // --- Knockback ---
            Rigidbody rb = other.attachedRigidbody;
            if (rb != null)
            {
                Vector3 knockDir = (other.transform.position - transform.position).normalized;
                rb.AddForce(knockDir * knockbackForce, ForceMode.Impulse);
            }
        }

        // Destroy bullet after collision
        Destroy(gameObject);
    }
}
