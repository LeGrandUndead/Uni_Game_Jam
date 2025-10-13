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
    public float rotationSpeed = 6f;
    public float stopDistance = 1.5f;

    [Header("Behavior")]
    public float chaseRadius = 10f;
    public float wanderRadius = 5f;
    public float wanderInterval = 3f;
    public float smoothAcceleration = 5f;

    [Header("Avoidance")]
    public float personalSpace = 2f;
    public float separationStrength = 5f;

    [Header("Abilities")]
    public float jumpForce = 5f;
    public float dashForce = 5f;
    public float dashChance = 0.2f;
    public float dashCooldown = 5f;

    private Rigidbody rb;
    private Vector3 wanderDirection;
    private float lastWanderTime;
    private float lastDashTime;
    private bool isAggro = false;
    private float currentSpeed;
    private Vector3 dashVelocity = Vector3.zero;

    public static List<Pursuit> allEnemies = new List<Pursuit>();

    void OnEnable() => allEnemies.Add(this);
    void OnDisable() => allEnemies.Remove(this);

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

        ChooseNewWanderDirection();
    }

    void Update()
    {
        if (player == null) return;

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
        Vector3 offset = new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f));
        Vector3 targetPos = player.position + offset;

        Vector3 separation = Vector3.zero;
        foreach (var other in allEnemies)
        {
            if (other == this) continue;
            Vector3 diff = transform.position - other.transform.position;
            float dist = diff.magnitude;
            if (dist < personalSpace && dist > 0.01f)
                separation += diff.normalized / dist;
        }

        Vector3 moveDir = (targetPos - transform.position).normalized * currentSpeed + separation * separationStrength;

        moveDir += dashVelocity;                      
        dashVelocity = Vector3.Lerp(dashVelocity, Vector3.zero, Time.deltaTime * 2f);

        moveDir.y = rb.velocity.y;
        rb.velocity = Vector3.Lerp(rb.velocity, moveDir, Time.deltaTime * smoothAcceleration);

        Vector3 flatDir = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        if (flatDir.sqrMagnitude > 0.01f)
        {
            Quaternion targetRot = Quaternion.LookRotation(flatDir.normalized);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * rotationSpeed);
        }
    }

    void Wander()
    {
        if (Time.time - lastWanderTime > wanderInterval)
            ChooseNewWanderDirection();

        Vector3 desired = wanderDirection * baseSpeed;
        desired.y = rb.velocity.y;
        rb.velocity = Vector3.Lerp(rb.velocity, desired, Time.deltaTime * smoothAcceleration);

        Vector3 flatDir = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        if (flatDir.sqrMagnitude > 0.01f)
        {
            Quaternion targetRot = Quaternion.LookRotation(flatDir.normalized);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * rotationSpeed);
        }
    }

    void ChooseNewWanderDirection()
    {
        Vector2 rand = Random.insideUnitCircle.normalized;
        wanderDirection = new Vector3(rand.x, 0f, rand.y);
        lastWanderTime = Time.time;
    }

    void TryRandomAbilities()
    {
        // Dash
        if (Time.time - lastDashTime > dashCooldown && Random.value < dashChance)
        {
            Vector3 dashDir = (player.position - transform.position).normalized;
            dashDir.y = 0f;
            dashVelocity += dashDir * dashForce;
            lastDashTime = Time.time;
        }

        // Jump occasionally
        if (Random.value < 0.01f && Mathf.Abs(rb.velocity.y) < 0.1f)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }
}
