using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Jump : MonoBehaviour
{
    [Header("Jump Settings")]
    public float jumpForce = 8f;
    public float fallMultiplier = 3.5f;  // was 2.5f → faster fall
    public float lowJumpMultiplier = 2.5f; // was 2f → snappier release
    public float coyoteTime = 0.15f;       // short buffer after leaving ground

    [Header("Ground Detection")]
    public float groundOffset = 1.0f;
    public float groundCheckDistance = 1.2f;
    public LayerMask groundLayer;

    private Rigidbody rb;
    private bool isGrounded;
    private float lastGroundedTime;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    void Update()
    {
        CheckGround();

        // Save the last time we were grounded
        if (isGrounded)
            lastGroundedTime = Time.time;

        // Jump input
        if (Input.GetKeyDown(KeyCode.Space) && Time.time - lastGroundedTime <= coyoteTime)
        {
            JumpAction();
        }

        ApplyBetterGravity();

        // Visual debug
        Debug.DrawRay(transform.position + Vector3.down * groundOffset, Vector3.down * groundCheckDistance, isGrounded ? Color.green : Color.red);
    }

    void CheckGround()
    {
        Vector3 origin = transform.position + Vector3.down * groundOffset;
        isGrounded = Physics.Raycast(origin, Vector3.down, groundCheckDistance, groundLayer);
    }

    void JumpAction()
    {
        Debug.Log("<color=green>Jump!</color>");
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    void ApplyBetterGravity()
    {
        if (rb.velocity.y < 0)
        {
            rb.velocity += Vector3.up * Physics.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }
        else if (rb.velocity.y > 0 && !Input.GetKey(KeyCode.Space))
        {
            rb.velocity += Vector3.up * Physics.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = isGrounded ? Color.green : Color.red;
        Gizmos.DrawLine(transform.position + Vector3.down * groundOffset, transform.position + Vector3.down * (groundOffset + groundCheckDistance));
    }
}
