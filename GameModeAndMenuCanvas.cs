using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Threading;
using UnityEngine.EventSystems;

public enum GameStartMode
{
    None = 0,
    Day = 1,
    Night = 2,
    Rainy = 3
}

public class GameModeSettings : MonoBehaviour
{
    public static GameModeSettings Instance;
    public GameStartMode SelectedMode = GameStartMode.None;
    public bool MenuConsumed = false;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void ResetStatics() => Instance = null;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
}

public class MenuCanvasBootstrap : MonoBehaviour
{
    [SerializeField] private string sceneToLoad = "scene_Menu";

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Create()
    {
        if (GameModeSettings.Instance == null)
            new GameObject("[GameModeSettings]").AddComponent<GameModeSettings>();

        var go = new GameObject("[MenuCanvasBootstrap]");
        go.AddComponent<MenuCanvasBootstrap>();
        Object.DontDestroyOnLoad(go);

        SceneInitializer.EnsureRunner();
    }

    void Start()
    {
        if (!GameModeSettings.Instance.MenuConsumed) Time.timeScale = 0f;
        BuildCanvasUI();
    }

    private void BuildCanvasUI()
    {
        if (GameModeSettings.Instance.MenuConsumed) return;

        if (FindObjectOfType<EventSystem>() == null)
        {
            var es = new GameObject("EventSystem");
            es.AddComponent<EventSystem>();
            es.AddComponent<StandaloneInputModule>();
            DontDestroyOnLoad(es);
        }

        // ===== Canvas =====
        var canvasGO = new GameObject("MenuCanvas");
        Object.DontDestroyOnLoad(canvasGO);
        var canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        var scaler = canvasGO.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        canvasGO.AddComponent<GraphicRaycaster>();

        // ===== Left Sidebar =====
        const float sidebarWidth = 640f;     // tweak if you want more room
        const float padding = 18f;
        const float buttonHeight = 72f;      // taller buttons so text breathes

        var panelGO = new GameObject("LeftSidebar");
        panelGO.transform.SetParent(canvasGO.transform, false);
        var image = panelGO.AddComponent<Image>();
        image.color = new Color(0f, 0f, 0f, 0.6f);

        var panelRT = panelGO.GetComponent<RectTransform>();
        panelRT.anchorMin = new Vector2(0f, 0f);
        panelRT.anchorMax = new Vector2(0f, 1f);
        panelRT.pivot     = new Vector2(0f, 0.5f);
        panelRT.sizeDelta = new Vector2(sidebarWidth, 0f);
        panelRT.anchoredPosition = Vector2.zero;

        // Built-in font (reliable)
        Font builtin = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

        // ===== Title =====
        var titleGO = new GameObject("Title");
        titleGO.transform.SetParent(panelGO.transform, false);
        var title = titleGO.AddComponent<Text>();
        title.text = "Select Game Mode";
        title.alignment = TextAnchor.MiddleLeft;
        title.fontSize = 40;
        title.color = Color.white;
        title.font = builtin;
        title.horizontalOverflow = HorizontalWrapMode.Overflow;
        title.verticalOverflow   = VerticalWrapMode.Truncate;

        var titleRT = titleGO.GetComponent<RectTransform>();
        titleRT.anchorMin = new Vector2(0f, 1f);
        titleRT.anchorMax = new Vector2(0f, 1f);
        titleRT.pivot     = new Vector2(0f, 1f);
        titleRT.sizeDelta = new Vector2(sidebarWidth - (padding * 2f), 84f);
        titleRT.anchoredPosition = new Vector2(padding, -padding);

        // ===== Button Column =====
        var columnGO = new GameObject("ButtonColumn");
        columnGO.transform.SetParent(panelGO.transform, false);
        var columnRT = columnGO.AddComponent<RectTransform>();
        columnRT.anchorMin = new Vector2(0f, 1f);
        columnRT.anchorMax = new Vector2(0f, 1f);
        columnRT.pivot     = new Vector2(0f, 1f);
        columnRT.anchoredPosition = new Vector2(padding, -padding - 90f);

        var vlg = columnGO.AddComponent<VerticalLayoutGroup>();
        vlg.childAlignment = TextAnchor.UpperLeft;
        vlg.spacing = 12f;
        vlg.padding = new RectOffset(0, 0, 0, 0);
        vlg.childControlWidth = true;
        vlg.childControlHeight = false;   // important: we control height
        vlg.childForceExpandWidth = false;
        vlg.childForceExpandHeight = false;

        var csf = columnGO.AddComponent<ContentSizeFitter>();
        csf.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
        csf.verticalFit   = ContentSizeFitter.FitMode.PreferredSize;

        // Helper to make a single-line, best-fit, left-aligned button
        UnityEngine.UI.Button CreateButton(string name, string labelText, System.Action onClick)
        {
            var btnGO = new GameObject(name);
            btnGO.transform.SetParent(columnGO.transform, false);

            var img = btnGO.AddComponent<Image>();
            img.color = new Color(1f, 1f, 1f, 0.92f);

            var rt = btnGO.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(sidebarWidth - (padding * 2f), buttonHeight);

            var le = btnGO.AddComponent<LayoutElement>();
            le.preferredWidth  = sidebarWidth - (padding * 2f);
            le.preferredHeight = buttonHeight;

            var btn = btnGO.AddComponent<Button>();

            var textGO = new GameObject("Text");
            textGO.transform.SetParent(btnGO.transform, false);
            var label = textGO.AddComponent<Text>();
            label.text = labelText;
            label.alignment = TextAnchor.MiddleLeft;
            label.fontSize = 28;                  // starting size
            label.color = Color.black;
            label.font = builtin;

            // >>> ensure full name shows on ONE line <<<
            label.resizeTextForBestFit = true;     // auto shrink if needed
            label.resizeTextMinSize = 14;
            label.resizeTextMaxSize = 28;
            label.horizontalOverflow = HorizontalWrapMode.Overflow; // no wrapping
            label.verticalOverflow   = VerticalWrapMode.Truncate;   // clip if too tall
            label.supportRichText = false;

            var trt = textGO.GetComponent<RectTransform>();
            trt.anchorMin = new Vector2(0f, 0f);
            trt.anchorMax = new Vector2(1f, 1f);
            trt.pivot     = new Vector2(0f, 0.5f);
            trt.offsetMin = new Vector2(16f, 0f);   // left padding
            trt.offsetMax = new Vector2(-16f, 0f);  // right padding

            btn.onClick.AddListener(() => onClick());
            return btn;
        }

        // Buttons with your requested labels
        CreateButton("BtnDay",   "Day",   () => StartWithMode(GameStartMode.Day));
        CreateButton("BtnNight", "Night", () => StartWithMode(GameStartMode.Night));
        CreateButton("BtnRainy", "Rainy", () => StartWithMode(GameStartMode.Rainy));

        // ===== Hint =====
        var hintGO = new GameObject("Hint");
        hintGO.transform.SetParent(panelGO.transform, false);
        var hint = hintGO.AddComponent<Text>();
        hint.text = "Tip: Add your entry scene to Build Settings. Target scene set in MenuCanvasBootstrap.sceneToLoad.";
        hint.alignment = TextAnchor.LowerLeft;
        hint.fontSize = 16;
        hint.color = Color.white;
        hint.font = builtin;
        hint.horizontalOverflow = HorizontalWrapMode.Overflow;
        hint.verticalOverflow   = VerticalWrapMode.Truncate;

        var hintRT = hintGO.GetComponent<RectTransform>();
        hintRT.anchorMin = new Vector2(0f, 0f);
        hintRT.anchorMax = new Vector2(0f, 0f);
        hintRT.pivot     = new Vector2(0f, 0f);
        hintRT.sizeDelta = new Vector2(sidebarWidth - (padding * 2f), 60f);
        hintRT.anchoredPosition = new Vector2(padding, padding);
    }

    private void StartWithMode(GameStartMode mode)
    {
        GameModeSettings.Instance.SelectedMode = mode;
        GameModeSettings.Instance.MenuConsumed = true;
        Time.timeScale = 1f;
        SceneManager.LoadScene(sceneToLoad);
    }
}
