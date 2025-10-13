using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(CapsuleCollider))]
public class Pursuit : MonoBehaviour
{
    [Header("Target & Stats")]
    public Transform player;
    public float baseSpeed = 3f;
    public float maxSpeed = 6f;
    public float rotationSpeed = 5f;
    public float stopDistance = 1.5f;

    [Header("Movement Behavior")]
    public float wanderRadius = 5f;
    public float wanderInterval = 3f;
    public float chaseRadius = 8f;
    public float smoothAcceleration = 2f;

    [Header("Jump / Dash / Teleport")]
    public float jumpForce = 5f;
    public float dashForce = 5f;
    public float dashChance = 0.2f;
    public float dashCooldown = 5f;
    public float teleportDistance = 5f;
    public float teleportChance = 0.1f;
    public float teleportCooldown = 10f;

    private Rigidbody rb;
    private CapsuleCollider capsule;

    private float lifeTimer = 0f;
    private float currentSpeed = 0f;
    private Vector3 wanderDirection;
    private float lastWanderTime;
    private float lastDashTime;
    private float lastTeleportTime;

    private bool isAggro = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        capsule = GetComponent<CapsuleCollider>();
        rb.freezeRotation = true;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

        ChooseNewWanderDirection();

        // Ignore collisions with other enemies
        foreach (var col in FindObjectsOfType<Pursuit>())
        {
            if (col != this)
                Physics.IgnoreCollision(GetComponent<Collider>(), col.GetComponent<Collider>());
        }
    }

    void Update()
    {
        lifeTimer += Time.deltaTime;
        float targetSpeed = Mathf.Lerp(baseSpeed, maxSpeed, lifeTimer / 60f);
        currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, Time.deltaTime * smoothAcceleration);

        TryRandomAbilities();

        float distanceToPlayer = player != null ? Vector3.Distance(transform.position, player.position) : Mathf.Infinity;

        if (distanceToPlayer <= chaseRadius || isAggro)
        {
            isAggro = true;
            ChasePlayer();
        }
        else
        {
            Wander();
        }
    }

    void ChasePlayer()
    {
        Vector3 direction = (player.position - transform.position);
        direction.y = 0;
        if (direction.magnitude < stopDistance) return;

        SmoothRotate(direction);

        Vector3 move = direction.normalized * currentSpeed * Time.deltaTime;
        MoveWithCollision(move);
    }

    void Wander()
    {
        if (Time.time - lastWanderTime > wanderInterval)
            ChooseNewWanderDirection();

        SmoothRotate(wanderDirection);

        Vector3 move = wanderDirection * currentSpeed * Time.deltaTime;
        MoveWithCollision(move);
    }

    void ChooseNewWanderDirection()
    {
        Vector2 randCircle = Random.insideUnitCircle.normalized;
        wanderDirection = new Vector3(randCircle.x, 0, randCircle.y);
        lastWanderTime = Time.time;
    }

    void SmoothRotate(Vector3 direction)
    {
        if (direction.magnitude < 0.1f) return;
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
    }

    void TryRandomAbilities()
    {
        if (Time.time - lastDashTime > dashCooldown && Random.value < dashChance)
        {
            Vector3 dashDir = player != null ? (player.position - transform.position).normalized : wanderDirection;
            dashDir.y = 0;
            rb.AddForce(dashDir * dashForce, ForceMode.Impulse);
            lastDashTime = Time.time;
        }

        if (Time.time - lastTeleportTime > teleportCooldown && Random.value < teleportChance)
        {
            Vector3 randomDir = Random.insideUnitSphere;
            randomDir.y = 0;
            randomDir.Normalize();
            Vector3 targetPos = transform.position + randomDir * teleportDistance;

            // Keep teleporting above ground
            RaycastHit hit;
            if (Physics.Raycast(targetPos + Vector3.up * 5f, Vector3.down, out hit, 10f, LayerMask.GetMask("Environment")))
                transform.position = hit.point + Vector3.up * 0.5f;
            else
                transform.position = targetPos;

            lastTeleportTime = Time.time;
        }

        // Random jump
        if (Random.value < 0.01f && IsGrounded())
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    void MoveWithCollision(Vector3 move)
    {
        Vector3 bottom = transform.position + capsule.center - Vector3.up * (capsule.height / 2 - capsule.radius);
        Vector3 top = bottom + Vector3.up * (capsule.height - capsule.radius * 2);

        RaycastHit hit;
        if (Physics.CapsuleCast(bottom, top, capsule.radius, move.normalized, out hit, move.magnitude, LayerMask.GetMask("Environment")))
        {
            // Slide along walls
            Vector3 slide = Vector3.ProjectOnPlane(move, hit.normal);
            rb.MovePosition(transform.position + slide);
        }
        else
        {
            rb.MovePosition(transform.position + move);
        }
    }

    bool IsGrounded()
    {
        Vector3 origin = transform.position + Vector3.up * 0.1f;
        return Physics.Raycast(origin, Vector3.down, 0.2f, LayerMask.GetMask("Environment"));
    }
}
