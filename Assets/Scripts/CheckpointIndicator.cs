using System.Collections;
using TMPro;
using UnityEngine;

public class CheckpointIndicator : MonoBehaviour
{
    public static CheckpointIndicator Instance;

    public float fadeDuration = 1f;
    public float displayTime = 5f;

    private TextMeshProUGUI label;
    private CanvasGroup canvasGroup;

    private void Awake()
    {
        Instance = this;

        label = GetComponent<TextMeshProUGUI>();

        canvasGroup = gameObject.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        gameObject.SetActive(false);
        canvasGroup.alpha = 0;
    }

    public void ShowIndicator()
    {
        gameObject.SetActive(true);
        StopAllCoroutines();
        StartCoroutine(Fade());
    }

    private IEnumerator Fade()
    {
        gameObject.SetActive(true);

        for (float fadeInTime = 0; fadeInTime < fadeDuration; fadeInTime += Time.deltaTime)
        {
            canvasGroup.alpha = fadeInTime / fadeDuration;
            yield return null;
        }
        canvasGroup.alpha = 1;

        yield return new WaitForSeconds(displayTime);

        for (float fadeOutTime = 0; fadeOutTime < fadeDuration; fadeOutTime += Time.deltaTime)
        {
            canvasGroup.alpha = 1 - (fadeOutTime / fadeDuration);
            yield return null;
        }
        canvasGroup.alpha = 0;

        gameObject.SetActive(false);
    }
}
