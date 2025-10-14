using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Jump : MonoBehaviour
{
    [Header("Jump Settings")]
    public float jumpForce = 8f;
    public float fallMultiplier = 3.5f;
    public float lowJumpMultiplier = 2.5f;
    public float coyoteTime = 0.15f;

    [Header("Ground Detection")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.3f;
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

        if (isGrounded)
            lastGroundedTime = Time.time;

        if (Input.GetKeyDown(KeyCode.Space) && Time.time - lastGroundedTime <= coyoteTime)
        {
            JumpAction();
        }

        ApplyBetterGravity();

        GizmosDrawSphere();
    }

    void CheckGround()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundLayer);
    }

    void JumpAction()
    {
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    void ApplyBetterGravity()
    {
        if (rb.velocity.y < 0)
            rb.velocity += Vector3.up * Physics.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        else if (rb.velocity.y > 0 && !Input.GetKey(KeyCode.Space))
            rb.velocity += Vector3.up * Physics.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
    }

    void GizmosDrawSphere()
    {
        #if UNITY_EDITOR
                if (groundCheck != null)
                    UnityEditor.Handles.color = isGrounded ? Color.green : Color.red;
                    UnityEditor.Handles.DrawWireDisc(groundCheck.position, Vector3.up, groundCheckRadius);
        #endif
    }
}
