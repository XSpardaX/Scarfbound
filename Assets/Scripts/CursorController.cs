using UnityEngine;
using UnityEngine.SceneManagement;

public static class CursorController
{
    private const string MainMenuSceneName = "Main Menu";

    public static void ApplyForActiveScene()
    {
        ApplyForScene(SceneManager.GetActiveScene().name);
    }

    public static void ApplyForScene(string sceneName)
    {
        if (sceneName == MainMenuSceneName)
            ApplyMenu();
        else
            ApplyGameplay();
    }

    public static void ApplyMenu()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public static void ApplyGameplay()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public static void ApplyUnlocked()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
