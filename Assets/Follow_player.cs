using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follow_player : MonoBehaviour
{
    public Transform player;
    public Vector3 offset = new Vector3(2f, 3f, -5f);
    public Vector3 runOffset = new Vector3(0f, 0f, -1.5f);
    public float smoothSpeed = 10f;
    public float rotationSpeed = 5f;
    public bool changeFOV = true;
    public float normalFOV = 60f;
    public float runFOV = 75f;
    public float fovLerpSpeed = 5f;
    public LayerMask collisionLayers;

    private float yaw = 0f;
    private float pitch = 5f;
    private float cameraRadius = 0.3f;

    void LateUpdate()
    {
        if (player == null) return;

        yaw += Input.GetAxis("Mouse X") * rotationSpeed;
        pitch -= Input.GetAxis("Mouse Y") * rotationSpeed;
        pitch = Mathf.Clamp(pitch, -20f, 20f);
        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);

        Vector3 currentOffset = offset;
        Movement playerMovement = player.GetComponent<Movement>();
        if (playerMovement != null && playerMovement.isRunning)
            currentOffset += runOffset;

        Vector3 desiredPosition = player.position + rotation * currentOffset;

        Vector3 dir = desiredPosition - player.position;
        float dist = dir.magnitude;
        RaycastHit hit;

        if (Physics.SphereCast(player.position, cameraRadius, dir.normalized, out hit, dist, collisionLayers))
        {
            desiredPosition = hit.point - dir.normalized * cameraRadius;
        }

        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);

        transform.LookAt(player.position + Vector3.up * 0.8f);

        if (changeFOV && Camera.main != null && playerMovement != null)
        {
            float targetFOV = playerMovement.isRunning ? runFOV : normalFOV;
            Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, targetFOV, Time.deltaTime * fovLerpSpeed);
        }
    }
}
