using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(CapsuleCollider))]
public class Movement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 2f;
    public float runSpeed = 6f;
    public float acceleration = 10f;
    public float rotationSpeed = 10f;

    [Header("Dash Settings")]
    public float dashDistance = 8f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 2f;

    [Header("Camera Reference")]
    public Transform cameraTransform;

    [HideInInspector] public bool isRunning = false;

    private Rigidbody rb;
    private Vector3 movementDirection;
    private bool canDash = true;
    private bool isDashing = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
    }

    void Update()
    {
        HandleInput();
        HandleDash();
    }

    void FixedUpdate()
    {
        if (!isDashing)
            MovePlayer();
    }

    void HandleInput()
    {
        movementDirection = Vector3.zero;
        if (Input.GetKey(KeyCode.Z)) movementDirection += Vector3.forward;
        if (Input.GetKey(KeyCode.S)) movementDirection += Vector3.back;
        if (Input.GetKey(KeyCode.Q)) movementDirection += Vector3.left;
        if (Input.GetKey(KeyCode.D)) movementDirection += Vector3.right;
        movementDirection = movementDirection.normalized;

        if (Input.GetKeyDown(KeyCode.LeftShift) && movementDirection.magnitude > 0.1f)
            isRunning = !isRunning;

        if (movementDirection.magnitude < 0.1f)
            isRunning = false;
    }

    void MovePlayer()
    {
        if (movementDirection.magnitude < 0.1f)
        {
            rb.velocity = new Vector3(0, rb.velocity.y, 0);
            return;
        }

        Vector3 moveDir = cameraTransform.TransformDirection(movementDirection);
        moveDir.y = 0f;
        moveDir.Normalize();

        float speed = isRunning ? runSpeed : walkSpeed;
        Vector3 desiredVelocity = moveDir * speed;

        RaycastHit hit;
        if (Physics.CapsuleCast(
            rb.position + Vector3.up * 0.5f,
            rb.position + Vector3.up * 1.8f,
            0.5f,
            moveDir,
            out hit,
            speed * Time.fixedDeltaTime,
            LayerMask.GetMask("Environment")))
        {
            Vector3 wallNormal = hit.normal;
            Vector3 slideDir = Vector3.ProjectOnPlane(desiredVelocity, wallNormal);
            desiredVelocity.x = slideDir.x;
            desiredVelocity.z = slideDir.z;
        }

        desiredVelocity.y = rb.velocity.y; 
        rb.velocity = Vector3.Lerp(rb.velocity, desiredVelocity, acceleration * Time.fixedDeltaTime);

        Vector3 flatVel = new Vector3(desiredVelocity.x, 0, desiredVelocity.z);
        if (flatVel.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(flatVel);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
        }
    }

    void HandleDash()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl) && canDash)
        {
            Vector3 dashDir = movementDirection.magnitude > 0.1f ? cameraTransform.TransformDirection(movementDirection) : transform.forward;
            dashDir.y = 0f;
            StartCoroutine(Dash(dashDir.normalized));
        }
    }

    IEnumerator Dash(Vector3 dashDirection)
    {
        canDash = false;
        isDashing = true;

        float elapsed = 0f;
        float dashSpeed = dashDistance / dashDuration;
        float verticalVelocity = rb.velocity.y;

        while (elapsed < dashDuration)
        {
            rb.velocity = new Vector3(dashDirection.x * dashSpeed, verticalVelocity, dashDirection.z * dashSpeed);
            elapsed += Time.deltaTime;
            yield return null;
        }

        isDashing = false;

        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }
}
