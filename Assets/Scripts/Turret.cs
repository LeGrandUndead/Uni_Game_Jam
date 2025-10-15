using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(CapsuleCollider))]
public class Turret : MonoBehaviour
{
    [Header("Targeting & Shooting")]
    public Transform player;
    public float detectionRadius = 10f;
    public float fireRate = 1f;
    public GameObject projectilePrefab;
    public Transform shootPoint;

    [Header("Movement")]
    public float moveSpeed = 2f;
    public float acceleration = 5f;
    public float patrolRadius = 5f;
    public float patrolInterval = 3f;

    [Header("Separation")]
    public float personalSpace = 2f;
    public float separationStrength = 2f;

    private Rigidbody rb;
    private Vector3 targetPosition;
    private float lastPatrolTime;
    private float lastShotTime;
    private Vector3 moveMomentum = Vector3.zero;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

        ChooseNewPatrolPosition();
    }

    void Update()
    {
        PatrolMovement();
        HandleShooting();
    }

    void PatrolMovement()
    {
        if (Time.time - lastPatrolTime > patrolInterval)
            ChooseNewPatrolPosition();

        Vector3 direction = (targetPosition - transform.position);

        Turret[] others = FindObjectsOfType<Turret>();
        Vector3 separation = Vector3.zero;
        foreach (var other in others)
        {
            if (other == this) continue;
            Vector3 diff = transform.position - other.transform.position;
            float dist = diff.magnitude;
            if (dist < personalSpace && dist > 0.01f)
                separation += diff.normalized / dist;
        }

        direction += separation * separationStrength;
        direction.y = 0f;

        Vector3 desiredVelocity = direction.normalized * moveSpeed;
        RaycastHit hit;
        if (Physics.CapsuleCast(
            rb.position + Vector3.up * 0.5f,
            rb.position + Vector3.up * 1.5f,
            0.5f,
            desiredVelocity.normalized,
            out hit,
            moveSpeed * Time.fixedDeltaTime,
            LayerMask.GetMask("Environment")
        ))
        {
            Vector3 slide = Vector3.ProjectOnPlane(desiredVelocity, hit.normal);
            desiredVelocity.x = slide.x;
            desiredVelocity.z = slide.z;
        }

        moveMomentum = Vector3.Lerp(moveMomentum, desiredVelocity, Time.deltaTime * acceleration);
        rb.velocity = new Vector3(moveMomentum.x, rb.velocity.y, moveMomentum.z);

        Vector3 flatVel = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        if (flatVel.sqrMagnitude > 0.01f)
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(flatVel), Time.deltaTime * 5f);
    }

    void ChooseNewPatrolPosition()
    {
        if (player == null) return;

        Vector2 rand = Random.insideUnitCircle * patrolRadius;
        targetPosition = player.position + new Vector3(rand.x, 0, rand.y);
        lastPatrolTime = Time.time;
    }

    void HandleShooting()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, detectionRadius);
        Transform closest = null;
        float minDist = Mathf.Infinity;

        foreach (var hit in hits)
        {
            if (hit.CompareTag("Enemy"))
            {
                float dist = Vector3.Distance(transform.position, hit.transform.position);
                if (dist < minDist)
                {
                    minDist = dist;
                    closest = hit.transform;
                }
            }
        }

        if (closest != null && Time.time - lastShotTime > 1f / fireRate)
        {
            ShootAt(closest.position);
            lastShotTime = Time.time;
        }
    }

    void ShootAt(Vector3 target)
    {
        if (projectilePrefab == null || shootPoint == null) return;

        GameObject bullet = Instantiate(projectilePrefab, shootPoint.position, Quaternion.identity);
        Vector3 dir = (target - shootPoint.position).normalized;

        Rigidbody rbBullet = bullet.GetComponent<Rigidbody>();
        if (rbBullet != null)
            rbBullet.velocity = dir * 20f;

        Vector3 flatDir = dir;
        flatDir.y = 0;
        if (flatDir.sqrMagnitude > 0.01f)
        {
            transform.rotation = Quaternion.LookRotation(flatDir);
            AudioManager.Instance.PlaySFX(AudioManager.Instance.turretAttack);
        }

        
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        if (player != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(player.position, patrolRadius);
        }

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, personalSpace);
    }
}
