using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.InputSystem.UI;

public class EndScreenManager : MonoBehaviour
{
    public static EndScreenManager Instance { get; private set; }

    [SerializeField] private GameObject endScreenPanel;
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private string mainMenuSceneName = "Main Menu";

    private bool buttonWired;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }

        Instance = this;

        if (endScreenPanel != null)
        {
            endScreenPanel.SetActive(false);
        }

        WireMainMenuButton();
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    public void ShowEndScreen()
    {
        WireMainMenuButton();

        if (endScreenPanel != null)
        {
            endScreenPanel.SetActive(true);
        }

        CursorController.ApplyUnlocked();
    }

    public void ReturnToMainMenu()
    {
        if (SfxManager.Instance != null)
        {
            SfxManager.Instance.Play(SfxIds.ButtonPress);
        }

        EndingState.isInEnding = false;
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuSceneName);
    }

    private void WireMainMenuButton()
    {
        if (buttonWired)
        {
            return;
        }

        if (mainMenuButton == null && endScreenPanel != null)
        {
            Transform buttonTransform = endScreenPanel.transform.Find("Overlay/QuitButton");
            if (buttonTransform == null)
            {
                buttonTransform = endScreenPanel.transform.Find("QuitButton");
            }

            if (buttonTransform != null)
            {
                mainMenuButton = buttonTransform.GetComponent<Button>();
            }
        }

        if (mainMenuButton == null)
        {
            return;
        }

        mainMenuButton.onClick.AddListener(ReturnToMainMenu);
        buttonWired = true;
    }
    
}
