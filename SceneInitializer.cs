
using UnityEngine;
using UnityEngine.SceneManagement;

[DefaultExecutionOrder(-1000)]
public class SceneInitializer : MonoBehaviour
{
    private static SceneInitializer _runner;

    public static void EnsureRunner()
    {
        if (_runner != null) return;
        var go = new GameObject("[SceneInitializer]");
        _runner = go.AddComponent<SceneInitializer>();
        GameObject.DontDestroyOnLoad(go);
        Debug.Log("[SceneInitializer] Runner created");
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void AutoCreate()
    {
        EnsureRunner();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void Start()
    {
        ApplyModeNow();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ApplyModeNow();
    }

    private void ApplyModeNow()
    {
        var settings = GameModeSettings.Instance;
        if (settings == null) return;

        switch (settings.SelectedMode)
        {
            case GameStartMode.Day:
                ApplyDay();
                break;
            case GameStartMode.Night:
                ApplyNight();
                break;
            case GameStartMode.Rainy:
                ApplyRainy();
                break;
            default:
                ApplyDay();
                break;
        }
    }

    private void ApplyDay()
    {
        EnsureBasicLighting(1.1f, new Color(0.9f, 0.95f, 1f));
       // RainSystemController.SetRainActive(false);
    }

    private void ApplyNight()
    {
        EnsureBasicLighting(0.2f, new Color(0.06f, 0.08f, 0.12f));
        RainSystemController.SetRainActive(false);
    }

    private void ApplyRainy()
    {
        EnsureBasicLighting(0.9f, new Color(0.6f, 0.65f, 0.7f));
        RainSystemController.SetRainActive(true);
    }

    private void EnsureBasicLighting(float lightIntensity, Color ambient)
    {
        RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
        RenderSettings.ambientLight = ambient;

        Light dir = GameObject.FindObjectOfType<Light>();
        if (dir != null && dir.type == LightType.Directional)
        {
            dir.intensity = lightIntensity;
            dir.color = Color.white;
        }
    }
}
