using UnityEngine;

public class FloatingUI : MonoBehaviour
{
    [Header("Floating Settings")]
    public float floatAmplitude = 5f; 
    public float floatSpeed = 1f;     
    public float rotationAmplitude = 2f;
    public float rotationSpeed = 1f;

    private Vector3 startPos;
    private Vector3 startRot;

    void Start()
    {
        startPos = transform.localPosition;
        startRot = transform.localEulerAngles;
    }

    void Update()
    {
        float time = Time.unscaledTime;
        float yOffset = Mathf.Sin(time * floatSpeed) * floatAmplitude;
        float zRot = Mathf.Sin(time * rotationSpeed) * rotationAmplitude;

        transform.localPosition = startPos + new Vector3(0, yOffset, 0);
        transform.localEulerAngles = startRot + new Vector3(0, 0, zRot);
    }
}
