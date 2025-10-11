using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pursuit : MonoBehaviour
{
    public Transform cible; // target (player)
    public float speed = 2f;
    public float rotationSpeed = 5f;
    public float stopDistance = 1.5f; // Distance to stop before touching

    private bool touchingPlayer = false;

    void Update()
    {
        if (cible == null) return;

        Vector3 direction = (cible.position - transform.position);
        direction.y = 0; // ignore vertical rotation

        float distance = direction.magnitude;

        if (!touchingPlayer && distance > stopDistance)
        {
            // Smooth rotation toward the target
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);

            // Move forward
            transform.position += transform.forward * speed * Time.deltaTime;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform == cible)
        {
            touchingPlayer = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.transform == cible)
        {
            touchingPlayer = false;
        }
    }
}