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

    [Header("Homing Settings")]
    public bool homingEnabled = true;
    public float detectionRadius = 10f;
    public float rotationSpeed = 10f;
    public string enemyTag = "Enemy";

    [Header("Bullet Type")]
    public bool isShockwave = false;

    private Transform target;

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        if (homingEnabled && !isShockwave)
        {
            if (target == null)
                FindClosestEnemy();

            if (target != null)
                RotateTowardTarget();
        }

        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    void FindClosestEnemy()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, detectionRadius);
        float closestDist = Mathf.Infinity;
        Transform closest = null;

        foreach (Collider hit in hits)
        {
            if (hit.CompareTag(enemyTag))
            {
                float dist = Vector3.Distance(transform.position, hit.transform.position);
                if (dist < closestDist)
                {
                    closestDist = dist;
                    closest = hit.transform;
                }
            }
        }

        if (closest != null)
            target = closest;
    }

    void RotateTowardTarget()
    {
        Vector3 direction = (target.position - transform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) return;

        if (other.CompareTag(enemyTag))
        {
            systeme_sante health = other.GetComponent<systeme_sante>();
            if (health != null)
            {
                if (isShockwave)
                {
                    Collider[] hits = Physics.OverlapSphere(transform.position, 5f);
                    foreach (Collider hit in hits)
                    {
                        if (hit.CompareTag(enemyTag))
                        {
                            systeme_sante h = hit.GetComponent<systeme_sante>();
                            if (h != null) h.TakeDamage(damage);

                            Rigidbody rb = hit.attachedRigidbody;
                            if (rb != null)
                            {
                                Vector3 dir = (hit.transform.position - transform.position).normalized;
                                rb.AddForce(dir * knockbackForce, ForceMode.Impulse);
                            }
                        }
                    }
                }
                else
                {
                    health.TakeDamage(damage);

                    Rigidbody rb = other.attachedRigidbody;
                    if (rb != null)
                    {
                        Vector3 knockDir = (other.transform.position - transform.position).normalized;
                        rb.AddForce(knockDir * knockbackForce, ForceMode.Impulse);
                    }
                }
            }

            Destroy(gameObject);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
