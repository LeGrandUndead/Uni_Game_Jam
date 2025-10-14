using UnityEngine;

public class Turret : MonoBehaviour
{
    public GameObject bulletPrefab;
    public float fireRate = 1f;
    public float detectionRadius = 10f;
    public string enemyTag = "Enemy";
    public float rotationSpeed = 5f; // how fast the turret rotates

    private float lastShotTime;
    private Transform currentTarget;

    void Update()
    {
        FindClosestEnemy();
        RotateTowardTarget();

        if (currentTarget != null && Time.time - lastShotTime > 1f / fireRate)
        {
            Shoot(currentTarget);
            lastShotTime = Time.time;
        }
    }

    void FindClosestEnemy()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, detectionRadius);
        float closestDist = Mathf.Infinity;
        currentTarget = null;

        foreach (var hit in hits)
        {
            if (hit.CompareTag(enemyTag))
            {
                float dist = Vector3.Distance(transform.position, hit.transform.position);
                if (dist < closestDist)
                {
                    closestDist = dist;
                    currentTarget = hit.transform;
                }
            }
        }
    }

    void RotateTowardTarget()
    {
        if (currentTarget == null) return;

        Vector3 direction = currentTarget.position - transform.position;
        direction.y = 0; // keep turret rotation on horizontal plane
        if (direction.sqrMagnitude < 0.01f) return;

        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    void Shoot(Transform target)
    {
        GameObject bullet = Instantiate(bulletPrefab, transform.position + Vector3.up, Quaternion.identity);
        bullet.transform.LookAt(target);
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        rb.AddForce(bullet.transform.forward * 20f, ForceMode.Impulse);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
