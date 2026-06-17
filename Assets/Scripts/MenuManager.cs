using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public GameObject controlsScreen;
    public GameObject mainPanel;
    public GameObject levelSelect;

    private bool buttonsWired;

    private void Awake()
    {
        ResolvePanelReferences();
    }

    private void Start()
    {
        CursorController.ApplyMenu();
        WireMenuButtons();
    }

    public void PlayGame(string sceneName)
    {
        if (GameState.Instance != null)
        {
            GameState.Instance.ResetForNewGame();
        }

        SceneManager.LoadScene(sceneName);
    }

    public void OpenPanel()
    {
        if (controlsScreen != null)
        {
            controlsScreen.SetActive(true);
            mainPanel.SetActive(false);
        }
    }

    public void OpenMain()
    {
        if (mainPanel != null)
        {
            mainPanel.SetActive(true);
            levelSelect.SetActive(false);
            if (controlsScreen != null)
                controlsScreen.SetActive(false);
        }
    }

    public void OpenLevelSelect()
    {
        if (levelSelect != null)
        {
            levelSelect.SetActive(true);
            mainPanel.SetActive(false);
        }
    }

    public void ClosePanel()
    {
        if (controlsScreen != null)
        {
            controlsScreen.SetActive(false);
            mainPanel.SetActive(true);
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    private void ResolvePanelReferences()
    {
        Canvas canvas = FindFirstObjectByType<Canvas>();
        if (canvas == null)
            return;

        if (mainPanel == null)
            mainPanel = canvas.transform.Find("Main Buttons")?.gameObject;

        if (levelSelect == null)
            levelSelect = canvas.transform.Find("Level Select")?.gameObject;

        if (controlsScreen == null)
        {
            foreach (Transform child in canvas.transform)
            {
                if (child.name != "Panel")
                    continue;

                if (child.Find("Button") != null)
                {
                    controlsScreen = child.gameObject;
                    break;
                }
            }
        }
    }

    private void WireMenuButtons()
    {
        if (buttonsWired)
            return;

        WireUnder(mainPanel != null ? mainPanel.transform : null, "Play", OpenLevelSelect);
        WireUnder(mainPanel != null ? mainPanel.transform : null, "Instructions", OpenPanel);
        WireUnder(mainPanel != null ? mainPanel.transform : null, "Quit", QuitGame);

        WireUnder(levelSelect != null ? levelSelect.transform : null, "Level 1", () => PlayGame("Level 1"));
        WireUnder(levelSelect != null ? levelSelect.transform : null, "Level 2", () => PlayGame("Level 2"));
        WireUnder(levelSelect != null ? levelSelect.transform : null, "Level 3", () => PlayGame("Level 3"));
        WireUnder(levelSelect != null ? levelSelect.transform : null, "Back", OpenMain);

        if (controlsScreen != null)
            WireUnder(controlsScreen.transform, "Button", ClosePanel);

        buttonsWired = true;
    }

    private void PlayButtonPressSfx()
    {
        if (SfxManager.Instance != null)
        {
            SfxManager.Instance.Play(SfxIds.ButtonPress);
        }
    }

    private void WireUnder(Transform parent, string childName, UnityEngine.Events.UnityAction action)
    {
        if (parent == null)
            return;

        Transform buttonTransform = parent.Find(childName);
        if (buttonTransform == null)
            return;

        Button button = buttonTransform.GetComponent<Button>();
        if (button != null)
            button.onClick.AddListener(() =>
            {
                PlayButtonPressSfx();
                action.Invoke();
            });
    }
}
