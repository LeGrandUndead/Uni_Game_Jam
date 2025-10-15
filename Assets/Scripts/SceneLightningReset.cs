using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLightingReset : MonoBehaviour
{
    [Header("Skybox")]
    public Material skyboxMaterial;

    void Awake()
    {
        if (!RenderSettings.skybox && skyboxMaterial != null)
            RenderSettings.skybox = skyboxMaterial;

        RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Skybox;
        RenderSettings.fog = false;

        RenderSettings.ambientIntensity = 1f;
        RenderSettings.ambientLight = Color.white;
        RenderSettings.reflectionIntensity = 1f;

    }
}
