using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class ScreenFader
{
    private const string FadeObjectName = "ScreenFade";

    private static Image fadeImage;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetStaticState()
    {
        fadeImage = null;
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void RegisterSceneCallbacks()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        fadeImage = null;
    }

    public static Image FadeImage => ResolveFadeImage();

    public static IEnumerator FadeOut(float duration)
    {
        yield return FadeTo(1f, duration);
    }

    public static IEnumerator FadeIn(float duration)
    {
        Image image = ResolveFadeImage();
        if (image != null)
        {
            EnsureFadeObjectActive(image);
            image.enabled = true;

            Color color = image.color;
            color.r = 0f;
            color.g = 0f;
            color.b = 0f;
            color.a = 1f;
            image.color = color;
        }

        yield return FadeTo(0f, duration);
    }

    private static IEnumerator FadeTo(float targetAlpha, float duration)
    {
        Image image = ResolveFadeImage();
        if (image == null)
            yield break;

        EnsureFadeObjectActive(image);
        image.enabled = true;

        Color color = image.color;
        color.r = 0f;
        color.g = 0f;
        color.b = 0f;
        float startAlpha = color.a;

        if (duration <= 0f)
        {
            color.a = targetAlpha;
            image.color = color;
            yield break;
        }

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            color.a = Mathf.Lerp(startAlpha, targetAlpha, elapsed / duration);
            image.color = color;
            yield return null;
        }

        color.a = targetAlpha;
        image.color = color;
    }

    private static void EnsureFadeObjectActive(Image image)
    {
        if (!image.gameObject.activeSelf)
            image.gameObject.SetActive(true);
    }

    private static Image ResolveFadeImage()
    {
        if (fadeImage != null)
            return fadeImage;

        LevelManager levelManager = Object.FindFirstObjectByType<LevelManager>();
        if (levelManager != null && levelManager.fadeImage != null)
        {
            fadeImage = levelManager.fadeImage;
            PrepareFadeImage(fadeImage);
            return fadeImage;
        }

        Canvas canvas = FindUICanvas();
        if (canvas == null)
            return null;

        Transform existingFade = canvas.transform.Find(FadeObjectName);
        if (existingFade != null)
        {
            fadeImage = existingFade.GetComponent<Image>();
            if (fadeImage != null)
            {
                PrepareFadeImage(fadeImage);
                return fadeImage;
            }
        }

        fadeImage = CreateFadeImage(canvas);
        return fadeImage;
    }

    private static void PrepareFadeImage(Image image)
    {
        image.raycastTarget = false;

        Color color = image.color;
        color.r = 0f;
        color.g = 0f;
        color.b = 0f;
        image.color = color;
    }

    private static Canvas FindUICanvas()
    {
        Canvas[] canvases = Object.FindObjectsByType<Canvas>(FindObjectsSortMode.None);
        foreach (Canvas canvas in canvases)
        {
            if (canvas.gameObject.name == "UICanvas")
                return canvas;
        }

        return Object.FindFirstObjectByType<Canvas>();
    }

    private static Image CreateFadeImage(Canvas canvas)
    {
        GameObject fadeObject = new GameObject(FadeObjectName);
        fadeObject.transform.SetParent(canvas.transform, false);
        fadeObject.transform.SetAsLastSibling();

        RectTransform rect = fadeObject.AddComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        Image image = fadeObject.AddComponent<Image>();
        image.color = new Color(0f, 0f, 0f, 0f);
        image.raycastTarget = false;

        return image;
    }
}
