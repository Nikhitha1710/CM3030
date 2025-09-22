using UnityEngine;

public class GameSettings : MonoBehaviour
{
    public enum EnvironmentMode { Day,Night, Rainy }

    public static GameSettings Instance { get; private set; }

    public EnvironmentMode SelectedMode = EnvironmentMode.Day;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
