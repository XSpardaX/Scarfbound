using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public GameObject controlsScreen;
    public GameObject mainPanel;
    public GameObject levelSelect;

    public void PlayGame(string sceneName)
    {
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
            mainPanel.SetActive(true );
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
