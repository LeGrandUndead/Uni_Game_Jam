using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerDeath : MonoBehaviour
{
    [Header("References")]
    public systeme_sante playerHealth; 
    public GameObject deathMenuCanvas;
    public GameObject UICanvas; 
    public RectTransform deathMenuPanel;

    [Header("Animation Settings")]
    public float slideDuration = 1f;
    public Vector2 hiddenPosition = new Vector2(0, -800);
    public Vector2 visiblePosition = Vector2.zero;

    private bool hasDied = false;

    void Update()
    {
        if (!hasDied && playerHealth != null && playerHealth.IsDead)
        {
            hasDied = true;
            StartCoroutine(ShowDeathMenu());
        }
    }

    private IEnumerator ShowDeathMenu()
    {

        if (deathMenuCanvas != null && !deathMenuCanvas.activeSelf)
        {
            deathMenuCanvas.SetActive(true);
            UICanvas.SetActive(false);
        }

        yield return null;

        if (deathMenuPanel != null)
            deathMenuPanel.anchoredPosition = hiddenPosition;

        float t = 0f;
        while (t < 1f)
        {
            t += Time.unscaledDeltaTime / slideDuration;
            float eased = Mathf.SmoothStep(0, 1, t);
            if (deathMenuPanel != null)
                deathMenuPanel.anchoredPosition = Vector2.Lerp(hiddenPosition, visiblePosition, eased);

            yield return null;
        }

        if (deathMenuPanel != null)
            deathMenuPanel.anchoredPosition = visiblePosition;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
