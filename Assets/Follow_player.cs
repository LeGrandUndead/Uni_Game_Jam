using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follow_player : MonoBehaviour
{
    public Transform player;
    public Vector3 offset = new Vector3(2f, 5f, -5f); // Closer & lower
    public float smoothSpeed = 10f;
    public float rotationSpeed = 5f;

    private float yaw = 0f;
    private float pitch = 5f;

    void LateUpdate()
    {
        // Mouse input
        yaw += Input.GetAxis("Mouse X") * rotationSpeed;
        pitch -= Input.GetAxis("Mouse Y") * rotationSpeed;

        // Arrow key input
        if (Input.GetKey(KeyCode.LeftArrow))
            yaw -= rotationSpeed * Time.deltaTime * 30f;
        if (Input.GetKey(KeyCode.RightArrow))
            yaw += rotationSpeed * Time.deltaTime * 30f;
        if (Input.GetKey(KeyCode.UpArrow))
            pitch += rotationSpeed * Time.deltaTime * 30f;
        if (Input.GetKey(KeyCode.DownArrow))
            pitch -= rotationSpeed * Time.deltaTime * 30f;

        pitch = Mathf.Clamp(pitch, -20f, 20f);

        // Rotation
        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);

        // Desired camera position
        Vector3 desiredPosition = player.position + rotation * offset;

        // Smooth movement
        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);

        // Look slightly above player's center
        transform.LookAt(player.position + Vector3.up * 0.8f);
    }
}
