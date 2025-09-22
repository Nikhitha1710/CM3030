using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    // optional: assign via inspector if you made a prefab; otherwise will FindObjectOfType
    public GameSettings settings;

    void Start()
    {
        if (settings == null) settings = FindObjectOfType<GameSettings>();
        if (settings == null)
        {
            var go = new GameObject("GameSettings");
            settings = go.AddComponent<GameSettings>();
        }
    }

    // Hook this to Button OnClick with int parameter: 0=Morning,1=Evening,2=Night,3=Rainy
    public void SelectAndStart(int modeIndex)
    {
        settings.SelectedMode = (GameSettings.EnvironmentMode)modeIndex;
        // load your driving scene
        SceneManager.LoadScene("scene_day");
    }
}
