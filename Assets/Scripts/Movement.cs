using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(CapsuleCollider))]
public class Movement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 2f;
    public float runSpeed = 6f;
    public float acceleration = 10f;

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
    private Vector3 dashMomentum = Vector3.zero;

    private bool isAZERTY = false; // Auto-detect QWERTY/AZERTY layout

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
        MovePlayer();
    }

    void HandleInput()
    {
        // --- Detect layout dynamically ---
        if (Input.anyKeyDown)
        {
            if (Input.GetKeyDown(KeyCode.W))
                isAZERTY = false; // QWERTY
            else if (Input.GetKeyDown(KeyCode.Z))
                isAZERTY = true;  // AZERTY
        }

        movementDirection = Vector3.zero;

        // Forward/back/strafe keys based on layout
        if (isAZERTY)
        {
            if (Input.GetKey(KeyCode.Z)) movementDirection += Vector3.forward;
            if (Input.GetKey(KeyCode.S)) movementDirection += Vector3.back;
            if (Input.GetKey(KeyCode.Q)) movementDirection += Vector3.left;
            if (Input.GetKey(KeyCode.D)) movementDirection += Vector3.right;
        }
        else
        {
            if (Input.GetKey(KeyCode.W)) movementDirection += Vector3.forward;
            if (Input.GetKey(KeyCode.S)) movementDirection += Vector3.back;
            if (Input.GetKey(KeyCode.A)) movementDirection += Vector3.left;
            if (Input.GetKey(KeyCode.D)) movementDirection += Vector3.right;
        }

        movementDirection = movementDirection.normalized;

        // Toggle running
        if (Input.GetKeyDown(KeyCode.LeftControl) && movementDirection.magnitude > 0.1f)
            isRunning = !isRunning;

        if (movementDirection.magnitude < 0.1f)
            isRunning = false;
    }

    void MovePlayer()
    {
        // Convert movement to camera-relative
        Vector3 camForward = cameraTransform.forward;
        camForward.y = 0f;
        camForward.Normalize();
        Vector3 camRight = cameraTransform.right;
        camRight.y = 0f;
        camRight.Normalize();

        Vector3 desiredMove = camForward * movementDirection.z + camRight * movementDirection.x;

        float speed = isRunning ? runSpeed : walkSpeed;
        Vector3 desiredVelocity = desiredMove * speed + dashMomentum;
        desiredVelocity.y = rb.velocity.y;

        // Wall sliding
        RaycastHit hit;
        if (Physics.CapsuleCast(
            rb.position + Vector3.up * 0.5f,
            rb.position + Vector3.up * 1.8f,
            0.5f,
            desiredMove.normalized,
            out hit,
            speed * Time.fixedDeltaTime,
            LayerMask.GetMask("Environment")))
        {
            Vector3 slideDir = Vector3.ProjectOnPlane(desiredVelocity, hit.normal);
            desiredVelocity.x = slideDir.x;
            desiredVelocity.z = slideDir.z;
        }

        // Smooth velocity
        rb.velocity = Vector3.Lerp(rb.velocity, desiredVelocity, acceleration * Time.fixedDeltaTime);

        // Player rotation = camera rotation
        Vector3 flatCamForward = camForward;
        if (flatCamForward.sqrMagnitude > 0.01f)
            transform.rotation = Quaternion.Euler(0, cameraTransform.eulerAngles.y, 0);

        // Gradually reduce dash momentum
        if (dashMomentum.magnitude > 0.01f)
            dashMomentum = Vector3.Lerp(dashMomentum, Vector3.zero, 5f * Time.fixedDeltaTime);
        else
            dashMomentum = Vector3.zero;
    }

    void HandleDash()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash)
        {
            Vector3 dashDir = movementDirection.magnitude > 0.1f ?
                              cameraTransform.TransformDirection(movementDirection) : cameraTransform.forward;
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

        // Keep momentum after dash
        dashMomentum = dashDirection * dashDistance / dashDuration;

        isDashing = false;
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }
}
