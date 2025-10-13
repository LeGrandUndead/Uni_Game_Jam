using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class onDeath : MonoBehaviour
{
    public systeme_sante Sante;
    public RawImage DeathScreenImage; // <-- Only death part

    private bool isDeadHandled = false;

    private void HandleDeath()
    {
        var rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.constraints = RigidbodyConstraints.None;
        }

        Destroy(gameObject, 5f);
    }

    private void ShowDeathScreen()
    {
        if (DeathScreenImage != null)
        {
            DeathScreenImage.gameObject.SetActive(true);
        }
    }

    void Update()
    {
        if (Sante != null && Sante.IsDead && !isDeadHandled)
        {
            isDeadHandled = true;

            HandleDeath();
            ShowDeathScreen();
        }
    }
}
