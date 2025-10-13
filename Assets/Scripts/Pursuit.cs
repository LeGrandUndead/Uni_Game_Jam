using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Pursuit : MonoBehaviour
{
    [Header("Target & Stats")]
    public Transform player;
    public float baseSpeed = 3f;
    public float maxSpeed = 6f;
    public float rotationSpeed = 6f;
    public float stopDistance = 1.5f;

    [Header("Behavior")]
    public float chaseRadius = 10f;
    public float wanderRadius = 5f;
    public float wanderInterval = 3f;
    public float smoothAcceleration = 5f;

    [Header("Formation & Avoidance")]
    public float personalSpace = 2f;
    public float separationStrength = 5f;

    [Header("Abilities")]
    public float jumpForce = 5f;
    public float dashForce = 5f;
    public float dashChance = 0.2f;
    public float dashCooldown = 5f;
    public float teleportDistance = 5f;
    public float teleportChance = 0.1f;
    public float teleportCooldown = 10f;

    private Rigidbody rb;
    private float lastWanderTime;
    private Vector3 wanderDirection;
    private float lastDashTime;
    private float lastTeleportTime;
    private bool isAggro = false;
    private float currentSpeed;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        rb.interpolation = RigidbodyInterpolation.Interpolate;

        // Ignore collisions with other enemies
        foreach (var col in FindObjectsOfType<Pursuit>())
        {
            if (col != this)
                Physics.IgnoreCollision(GetComponent<Collider>(), col.GetComponent<Collider>());
        }

        ChooseNewWanderDirection();
    }

    void Update()
    {
        if (player == null) return;

        // Dynamic speed: enemies get faster over time
        currentSpeed = Mathf.Lerp(baseSpeed, maxSpeed, Time.timeSinceLevelLoad / 60f);

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (distanceToPlayer <= chaseRadius || isAggro)
        {
            isAggro = true;
            ChasePlayer();
        }
        else
        {
            Wander();
        }

        TryRandomAbilities();
    }

    void ChasePlayer()
    {
        Vector3 target = player.position;

        // Apply separation from other enemies
        Vector3 separation = Vector3.zero;
        Pursuit[] others = FindObjectsOfType<Pursuit>();
        foreach (var other in others)
        {
            if (other == this) continue;
            Vector3 diff = transform.position - other.transform.position;
            float dist = diff.magnitude;
            if (dist < personalSpace && dist > 0.01f)
                separation += diff.normalized / dist;
        }

        Vector3 direction = (target - transform.position).normalized * currentSpeed + separation * separationStrength;
        direction.y = 0f;

        // Move smoothly with Rigidbody
        rb.velocity = Vector3.Lerp(rb.velocity, direction, Time.deltaTime * smoothAcceleration);

        // Rotate toward movement
        if (direction.sqrMagnitude > 0.1f)
        {
            Quaternion targetRot = Quaternion.LookRotation(direction.normalized);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * rotationSpeed);
        }
    }

    void Wander()
    {
        if (Time.time - lastWanderTime > wanderInterval)
            ChooseNewWanderDirection();

        Vector3 desired = wanderDirection * baseSpeed;
        desired.y = 0f;
        rb.velocity = Vector3.Lerp(rb.velocity, desired, Time.deltaTime * smoothAcceleration);

        if (rb.velocity.sqrMagnitude > 0.1f)
        {
            Quaternion targetRot = Quaternion.LookRotation(rb.velocity.normalized);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * rotationSpeed);
        }
    }

    void ChooseNewWanderDirection()
    {
        Vector2 rand = Random.insideUnitCircle.normalized;
        wanderDirection = new Vector3(rand.x, 0, rand.y);
        lastWanderTime = Time.time;
    }

    void TryRandomAbilities()
    {
        // Dash
        if (Time.time - lastDashTime > dashCooldown && Random.value < dashChance)
        {
            Vector3 dashDir = (player.position - transform.position).normalized;
            dashDir.y = 0;
            rb.AddForce(dashDir * dashForce, ForceMode.Impulse);
            lastDashTime = Time.time;
        }

        // Teleport
        if (Time.time - lastTeleportTime > teleportCooldown && Random.value < teleportChance)
        {
            Vector3 randomDir = Random.insideUnitSphere;
            randomDir.y = 0;
            randomDir.Normalize();
            transform.position += randomDir * teleportDistance;
            lastTeleportTime = Time.time;
        }

        // Jump occasionally
        if (Random.value < 0.01f)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }
}
