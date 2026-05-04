using UnityEngine;
using UnityEngine.UI;
using System.Collections;

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

    IEnumerator FadeIn()
    {
        if (fadeImage == null) yield break;

        float t = 1f;

        Color c = fadeImage.color;
        c.a = 1f;
        fadeImage.color = c;

        while (t > 0f)
        {
            t -= Time.deltaTime / fadeDuration;
            c.a = t;
            fadeImage.color = c;
            yield return null;
        }

        c.a = 0f;
        fadeImage.color = c;
    }
}