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
        rb.freezeRotation = true;
        ChooseNewWanderDirection();

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
        rb.MovePosition(transform.position + direction.normalized * currentSpeed * Time.deltaTime);
    }

    void Wander()
    {
        if (Time.time - lastWanderTime > wanderInterval)
        {
            ChooseNewWanderDirection();
        }

        SmoothRotate(wanderDirection);
        rb.MovePosition(transform.position + wanderDirection * currentSpeed * Time.deltaTime);
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
            transform.position += randomDir * teleportDistance;
            lastTeleportTime = Time.time;
        }

        if (Random.value < 0.01f)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }
}
