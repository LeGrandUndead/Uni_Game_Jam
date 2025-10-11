using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 2f;
    public float runSpeed = 5f;
    public float acceleration = 5f;
    public float dashDistance = 5f;
    public float dashCooldown = 2f;
    public float dashDuration = 0.2f;

    [Header("Camera Reference")]
    public Transform cameraTransform;

    private float currentSpeed = 0f;
    private float targetSpeed = 0f;
    private bool canDash = true;
    private bool isDashing = false;

    private Vector3 movementDirection;

    void Update()
    {
        if (!isDashing)
        {
            HandleMovementInput();
            MovePlayer();
        }

        HandleDash();
    }

    void HandleMovementInput()
    {
        targetSpeed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed;
        currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, acceleration * Time.deltaTime);

        movementDirection = Vector3.zero;

        if (Input.GetKey(KeyCode.Z)) movementDirection += Vector3.forward;
        if (Input.GetKey(KeyCode.S)) movementDirection += Vector3.back;
        if (Input.GetKey(KeyCode.Q)) movementDirection += Vector3.left;
        if (Input.GetKey(KeyCode.D)) movementDirection += Vector3.right;

        movementDirection = movementDirection.normalized;
    }

    void HandleDash()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl) && canDash)
        {
            // Calculate the dash direction relative to camera
            Vector3 dashDir = movementDirection.magnitude > 0.1f
                ? cameraTransform.TransformDirection(movementDirection)
                : transform.forward;

            dashDir.y = 0f;
            StartCoroutine(Dash(dashDir.normalized));
        }
    }

    IEnumerator Dash(Vector3 dashDirection)
    {
        canDash = false;
        isDashing = true;

        Vector3 startPosition = transform.position;
        Vector3 targetPosition = startPosition + dashDirection * dashDistance;

        float elapsed = 0f;

        while (elapsed < dashDuration)
        {
            transform.position = Vector3.Lerp(startPosition, targetPosition, elapsed / dashDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPosition;

        isDashing = false;
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    void MovePlayer()
    {
        if (movementDirection.magnitude > 0.1f)
        {
            // Align movement with camera direction
            Vector3 moveDir = cameraTransform.TransformDirection(movementDirection);
            moveDir.y = 0f;

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                Quaternion.LookRotation(moveDir),
                Time.deltaTime * 10f
            );

            transform.Translate(Vector3.forward * currentSpeed * Time.deltaTime);
        }
    }
}
