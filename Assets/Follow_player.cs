using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follow_player : MonoBehaviour
{
    [Header("References")]
    public Transform player;

    [Header("Camera Settings")]
    public Vector3 offset = new Vector3(2f, 3f, -4f); // over-the-shoulder position
    public float smoothSpeed = 10f;
    public float rotationSpeed = 5f;

    [Header("Distance Settings")]
    public float minDistance = 1.5f;   // Prevents camera being too close to player
    public float maxDistance = 6f;     // Maximum allowed distance (when zooming out from collision)
    public float collisionBuffer = 0.2f; // How far from obstacles camera stops

    [HideInInspector] public bool isDashing = false;

    private float yaw = 0f;
    private float pitch = 10f;
    private float currentDistance;
    private Vector3 desiredPosition;

    [Header("Collision Settings")]
    public LayerMask collisionMask; // Assign to everything except Player layer in Inspector

    void Start()
    {
        currentDistance = offset.magnitude;
    }

    void LateUpdate()
    {
        if (!player) return;

        HandleCameraInput();
        FollowPlayer();
    }

    void HandleCameraInput()
    {
        // Mouse input
        yaw += Input.GetAxis("Mouse X") * rotationSpeed;
        pitch -= Input.GetAxis("Mouse Y") * rotationSpeed;

        // Arrow key input
        if (Input.GetKey(KeyCode.LeftArrow)) yaw -= rotationSpeed * Time.deltaTime * 30f;
        if (Input.GetKey(KeyCode.RightArrow)) yaw += rotationSpeed * Time.deltaTime * 30f;
        if (Input.GetKey(KeyCode.UpArrow)) pitch += rotationSpeed * Time.deltaTime * 30f;
        if (Input.GetKey(KeyCode.DownArrow)) pitch -= rotationSpeed * Time.deltaTime * 30f;

        pitch = Mathf.Clamp(pitch, -15f, 25f);
    }

    void FollowPlayer()
    {
        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);

        // Desired camera position (before collision adjustments)
        Vector3 targetPos = player.position + rotation * offset;
        Vector3 direction = (targetPos - player.position).normalized;

        float targetDistance = offset.magnitude;
        float finalDistance = targetDistance;

        // Raycast to detect obstacles between player and camera
        if (Physics.Raycast(player.position + Vector3.up * 0.8f, direction, out RaycastHit hit, targetDistance, collisionMask))
        {
            finalDistance = Mathf.Clamp(hit.distance - collisionBuffer, minDistance, maxDistance);
        }

        desiredPosition = player.position + rotation * (offset.normalized * finalDistance);

        // Apply smoothing
        float effectiveSmooth = isDashing ? smoothSpeed * 0.5f : smoothSpeed;
        transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * effectiveSmooth);

        // Look slightly above player's center
        transform.LookAt(player.position + Vector3.up * 0.8f);
    }

    void OnDrawGizmosSelected()
    {
        if (player)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(player.position + Vector3.up * 0.8f, transform.position);
        }
    }
}
