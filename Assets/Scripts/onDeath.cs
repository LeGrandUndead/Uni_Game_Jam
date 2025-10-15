using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class onDeath : MonoBehaviour
{
    [Header("Enemy Health")]
    public systeme_sante Sante;

    private bool isDeadHandled = false;

    private void HandleDeath()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.constraints = RigidbodyConstraints.None;
        }

        Destroy(gameObject, 5f);
    }

    void Update()
    {
        if (Sante != null && Sante.IsDead && !isDeadHandled)
        {
            isDeadHandled = true;
            HandleDeath();
        }
    }
}
