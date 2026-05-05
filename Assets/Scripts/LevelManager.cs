using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    public Player player;
    public Image fadeImage;

    public float fadeDuration = 1.5f;

    private void Start()
    {
        if (player != null)
        {
            player.hasKey = false;
        }

        StartCoroutine(FadeIn());
    }

    private IEnumerator FadeIn()
    {
        if (fadeImage == null) yield break;

        float fadeProgress = 1f;

        Color fadeColor = fadeImage.color;
        fadeColor.a = 1f;
        fadeImage.color = fadeColor;

        while (fadeProgress > 0f)
        {
            fadeProgress -= Time.deltaTime / fadeDuration;
            fadeColor.a = fadeProgress;
            fadeImage.color = fadeColor;
            yield return null;
        }

        fadeColor.a = 0f;
        fadeImage.color = fadeColor;
    }
}
