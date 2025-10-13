using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Movement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 2f;
    public float runSpeed = 5f;
    public float acceleration = 10f;

    [Header("Dash Settings")]
    public float dashDistance = 5f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 2f;

    [Header("Camera Reference")]
    public Transform cameraTransform;

    private Rigidbody rb;
    private Vector3 movementInput;
    private float targetSpeed;
    private float currentSpeed;
    private bool canDash = true;
    private bool isDashing = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true; // keep upright
    }

    void Update()
    {
        HandleMovementInput();
        HandleDash();
    }

    void FixedUpdate()
    {
        if (!isDashing)
            MovePlayer();
    }

    void HandleMovementInput()
    {
        targetSpeed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed;
        currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, acceleration * Time.deltaTime);

        movementInput = Vector3.zero;
        if (Input.GetKey(KeyCode.Z)) movementInput += Vector3.forward;
        if (Input.GetKey(KeyCode.S)) movementInput += Vector3.back;
        if (Input.GetKey(KeyCode.Q)) movementInput += Vector3.left;
        if (Input.GetKey(KeyCode.D)) movementInput += Vector3.right;

        movementInput = movementInput.normalized;
    }

    void MovePlayer()
    {
        if (movementInput.magnitude > 0.1f)
        {
            Vector3 moveDir = cameraTransform.TransformDirection(movementInput);
            moveDir.y = 0f;

            // Rotate player smoothly
            Quaternion targetRotation = Quaternion.LookRotation(moveDir);
            rb.rotation = Quaternion.Slerp(rb.rotation, targetRotation, Time.fixedDeltaTime * 10f);

            // Move player
            Vector3 velocity = moveDir * currentSpeed;
            velocity.y = rb.velocity.y; // preserve vertical velocity
            rb.velocity = velocity;
        }
        else
        {
            rb.velocity = new Vector3(0, rb.velocity.y, 0);
        }
    }

    void HandleDash()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl) && canDash)
        {
            // Dash direction relative to camera
            Vector3 dashDir = movementInput.magnitude > 0.1f
                ? cameraTransform.TransformDirection(movementInput)
                : transform.forward;

            dashDir.y = 0f;

            // Prevent dashing directly into the camera
            Vector3 camToPlayer = (transform.position - cameraTransform.position).normalized;
            if (Vector3.Dot(dashDir, camToPlayer) < 0)
            {
                dashDir = Vector3.ProjectOnPlane(dashDir, camToPlayer).normalized;
            }

            StartCoroutine(Dash(dashDir));
        }
    }

    IEnumerator Dash(Vector3 dashDirection)
    {
        canDash = false;
        isDashing = true;

        float elapsed = 0f;
        float dashSpeed = dashDistance / dashDuration;

        while (elapsed < dashDuration)
        {
            Vector3 velocity = dashDirection * dashSpeed;
            velocity.y = rb.velocity.y; // preserve vertical
            rb.velocity = velocity;

            elapsed += Time.deltaTime;
            yield return null;
        }

        rb.velocity = new Vector3(0, rb.velocity.y, 0); // stop horizontal dash
        isDashing = false;

        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }
}
