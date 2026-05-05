using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public GameObject controlsScreen;

    public void PlayGame(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void OpenPanel()
    {
        if (controlsScreen != null)
        {
            controlsScreen.SetActive(true);
        }
    }

    public void ClosePanel()
    {
        if (controlsScreen != null)
        {
            controlsScreen.SetActive(false);
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
