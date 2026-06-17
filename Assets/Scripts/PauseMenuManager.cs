using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
#endif

// pause menu and level changes
public class PauseMenuManager : MonoBehaviour
{
    public static bool IsPaused { get; private set; }

    [SerializeField] private GameObject pausePanel;
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private string mainMenuSceneName = "Main Menu";

    private Transform pauseMenuRoot;

    // scene load setup
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Bootstrap()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        OnSceneLoaded(SceneManager.GetActiveScene(), LoadSceneMode.Single);
    }

    private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Time.timeScale = 1f;
        IsPaused = false;
        CheckpointManager.ResetRespawnState();
        DialogueState.isInDialogue = false;
        EndingState.isInEnding = false;
        CursorController.ApplyForScene(scene.name);
        EnsurePauseMenu();

        if (scene.name != "Main Menu")
        {
            CheckpointManager.EnsureExistsForScene();
        }
    }

    private static void EnsurePauseMenu()
    {
        if (SceneManager.GetActiveScene().name == "Main Menu")
            return;

        EnsureEventSystem();

        PauseMenuManager manager = FindFirstObjectByType<PauseMenuManager>(FindObjectsInactive.Include);
        if (manager == null)
        {
            Canvas canvas = FindUICanvas();
            if (canvas == null)
                return;

            manager = canvas.gameObject.AddComponent<PauseMenuManager>();
        }

        manager.Initialize();
    }

    private static void EnsureEventSystem()
    {
        if (FindFirstObjectByType<EventSystem>() != null)
            return;

        GameObject eventSystemObject = new GameObject("EventSystem");
        eventSystemObject.AddComponent<EventSystem>();
#if ENABLE_INPUT_SYSTEM
        eventSystemObject.AddComponent<InputSystemUIInputModule>();
#else
        eventSystemObject.AddComponent<StandaloneInputModule>();
#endif
    }

    private static Canvas FindUICanvas()
    {
        Canvas[] canvases = FindObjectsByType<Canvas>(FindObjectsSortMode.None);
        foreach (Canvas canvas in canvases)
        {
            if (canvas.gameObject.name == "UICanvas")
                return canvas;
        }

        return FindFirstObjectByType<Canvas>();
    }

    // find and wire pause UI
    private void Awake()
    {
        Initialize();
    }

    public void Initialize()
    {
        GameObject legacyPauseMenu = GameObject.Find("PauseMenu");
        if (legacyPauseMenu != null)
            Destroy(legacyPauseMenu);

        ResolveReferences();
        WireButtons();

        if (pausePanel != null)
            pausePanel.SetActive(false);
    }

    private void ResolveReferences()
    {
        if (pauseMenuRoot == null)
        {
            pauseMenuRoot = transform.Find("PauseMenuPanel");
            if (pauseMenuRoot == null)
                pauseMenuRoot = FindChildRecursive(transform, "PauseMenuPanel");
        }

        if (pauseMenuRoot == null)
        {
            GameObject prefab = Resources.Load<GameObject>("PauseMenuPanel");
            if (prefab != null)
            {
                GameObject instance = Instantiate(prefab, transform);
                instance.name = "PauseMenuPanel";
                pauseMenuRoot = instance.transform;
                pauseMenuRoot.SetAsLastSibling();
            }
        }

        if (pauseMenuRoot == null)
            return;

        if (pausePanel == null)
        {
            Transform overlay = pauseMenuRoot.Find("Overlay");
            pausePanel = overlay != null ? overlay.gameObject : pauseMenuRoot.gameObject;
        }

        Transform buttonRoot = pausePanel != null ? pausePanel.transform : pauseMenuRoot;
        if (resumeButton == null)
            resumeButton = FindButton(buttonRoot, "ResumeButton");
        if (restartButton == null)
            restartButton = FindButton(buttonRoot, "RestartButton");
        if (quitButton == null)
            quitButton = FindButton(buttonRoot, "QuitButton");
    }

    private static Transform FindChildRecursive(Transform parent, string childName)
    {
        foreach (Transform child in parent)
        {
            if (child.name == childName)
                return child;

            Transform nested = FindChildRecursive(child, childName);
            if (nested != null)
                return nested;
        }

        return null;
    }

    private static Button FindButton(Transform root, string buttonName)
    {
        Transform buttonTransform = root.Find(buttonName);
        if (buttonTransform == null)
            buttonTransform = FindChildRecursive(root, buttonName);

        return buttonTransform != null ? buttonTransform.GetComponent<Button>() : null;
    }

    private void WireButtons()
    {
        if (resumeButton != null)
        {
            resumeButton.onClick.RemoveListener(OnResumeButtonPressed);
            resumeButton.onClick.AddListener(OnResumeButtonPressed);
        }

        if (restartButton != null)
        {
            restartButton.onClick.RemoveListener(OnRestartButtonPressed);
            restartButton.onClick.AddListener(OnRestartButtonPressed);
        }

        if (quitButton != null)
        {
            quitButton.onClick.RemoveListener(OnQuitButtonPressed);
            quitButton.onClick.AddListener(OnQuitButtonPressed);
        }
    }

    private void PlayButtonPressSfx()
    {
        if (SfxManager.Instance != null)
        {
            SfxManager.Instance.Play(SfxIds.ButtonPress);
        }
    }

    private void OnResumeButtonPressed()
    {
        PlayButtonPressSfx();
        Resume();
    }

    private void OnRestartButtonPressed()
    {
        PlayButtonPressSfx();
        RestartLevel();
    }

    private void OnQuitButtonPressed()
    {
        PlayButtonPressSfx();
        QuitToMainMenu();
    }

    // escape and P key
    private void Update()
    {
        if (DialogueState.isInDialogue || EndingState.isInEnding)
            return;

        if (WasPausePressed())
            TogglePause();
    }

    private static bool WasPausePressed()
    {
#if ENABLE_INPUT_SYSTEM
        if (Keyboard.current != null)
        {
            if (Keyboard.current.escapeKey.wasPressedThisFrame || Keyboard.current.pKey.wasPressedThisFrame)
                return true;
        }
#endif
        return Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P);
    }

    public void TogglePause()
    {
        if (IsPaused)
            Resume();
        else
            Pause();
    }

    public void Pause()
    {
        if (pausePanel == null)
            ResolveReferences();

        if (pausePanel == null || IsPaused || DialogueState.isInDialogue || EndingState.isInEnding)
            return;

        IsPaused = true;

        if (pauseMenuRoot != null)
            pauseMenuRoot.SetAsLastSibling();

        pausePanel.SetActive(true);
        Time.timeScale = 0f;
        CursorController.ApplyUnlocked();
    }

    public void Resume()
    {
        if (pausePanel == null || !IsPaused)
            return;

        IsPaused = false;
        pausePanel.SetActive(false);
        Time.timeScale = 1f;
        CursorController.ApplyGameplay();
    }

    // restart or quit to menu
    public void RestartLevel()
    {
        Time.timeScale = 1f;
        IsPaused = false;
        CheckpointManager.ResetRespawnState();
        DialogueState.isInDialogue = false;
        EndingState.isInEnding = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void QuitToMainMenu()
    {
        Time.timeScale = 1f;
        IsPaused = false;
        CheckpointManager.ResetRespawnState();
        DialogueState.isInDialogue = false;
        EndingState.isInEnding = false;
        SceneManager.LoadScene(mainMenuSceneName);
    }
}
