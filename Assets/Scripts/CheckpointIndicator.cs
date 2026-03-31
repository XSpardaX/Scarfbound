using UnityEngine;
using TMPro;
using System.Collections;

public class CheckpointIndicator : MonoBehaviour
{
    public static CheckpointIndicator Instance;

    private TextMeshProUGUI tmp;
    private CanvasGroup canvasGroup;

    public float fadeDuration = 1f;
    public float displayTime = 5f;

    private void Awake()
    {
        Instance = this;
        tmp = GetComponent<TextMeshProUGUI>();

        canvasGroup = gameObject.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();

        gameObject.SetActive(false);
        canvasGroup.alpha = 0;
    }

    public void ShowIndicator()
    {
        gameObject.SetActive (true);
        StopAllCoroutines();
        StartCoroutine(Fade());
    }

    private IEnumerator Fade()
    {
        gameObject.SetActive(true);

        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            canvasGroup.alpha = t / fadeDuration;
            yield return null;
        }
        canvasGroup.alpha = 1;

        yield return new WaitForSeconds(displayTime);

        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            canvasGroup.alpha = 1 - (t / fadeDuration);
            yield return null;
        }
        canvasGroup.alpha = 0;

        gameObject.SetActive(false);
    }
}