using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Morsure : MonoBehaviour
{
    public float damage = 5f;
    public float playerKnockbackForce = 8f;
    public float enemyKnockbackForce = 3f;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Rigidbody rb = collision.rigidbody;
            if (rb != null)
            {
                Vector3 pushDir = (collision.transform.position - transform.position).normalized;
                pushDir.y = 0f;
                rb.AddForce(pushDir * enemyKnockbackForce, ForceMode.Impulse);
            }
            return;
        }

        systeme_sante targetHealth = collision.gameObject.GetComponent<systeme_sante>();
        if (targetHealth != null)
        {
            targetHealth.TakeDamage(damage);

            Rigidbody rb = collision.rigidbody;
            if (rb != null)
            {
                Vector3 pushDir = (collision.transform.position - transform.position).normalized;
                pushDir.y = 0f;
                rb.AddForce(pushDir * playerKnockbackForce, ForceMode.Impulse);
            }

            AudioManager.Instance.PlaySFX(AudioManager.Instance.enemyAttack);
        }
    }
}
