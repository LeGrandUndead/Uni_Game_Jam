using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class Barre_Sante : MonoBehaviour
{
    [SerializeField] private Slider Sliderdesante;
    [SerializeField] private systeme_sante Systemdesante;
    [SerializeField] private float smoothSpeed = 5f;
    private float targetFillAmount = 1f;


    private void HandleHealthChanged(float normalizedHealth)
    {
        targetFillAmount = Mathf.Clamp01(normalizedHealth);
        UpdateFillColor(normalizedHealth);
    }

    private void UpdateFillColor(float healthNormalized)
    {
        if (Sliderdesante.fillRect != null)
        {
            var fillImage = Sliderdesante.fillRect.GetComponent<Image>();
            if (fillImage != null)
            {
                fillImage.color = Color.Lerp(Color.red, Color.green, healthNormalized);
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        if (Sliderdesante == null || Systemdesante == null) return;

        Sliderdesante.minValue = 0f;
        Sliderdesante.maxValue = 1f;
        Sliderdesante.value = 1f;

        // Initialize with current health
        float initialHealth = Systemdesante.ObtenirSanteNormalisee();
        targetFillAmount = Mathf.Clamp01(initialHealth);
        UpdateFillColor(initialHealth);

        Systemdesante.OnChangedSante += HandleHealthChanged;
    }

    // Update is called once per frame
    void Update()
    {
        Sliderdesante.value = Mathf.Lerp(Sliderdesante.value, targetFillAmount, Time.deltaTime * smoothSpeed);
    }
}
