using System.Collections;
using TMPro;
using UnityEngine;

public class BossHintUI : MonoBehaviour
{
    public static BossHintUI Instance;

    public float fadeDuration = 0.4f;
    public float pulseDuration = 0.9f;
    public float pulseScaleMin = 0.94f;
    public float pulseScaleMax = 1.08f;
    public float pulseAlphaMin = 0.7f;

    private TextMeshProUGUI label;
    private CanvasGroup canvasGroup;
    private RectTransform rectTransform;
    private Vector3 baseScale;
    private Coroutine showRoutine;

    private void Awake()
    {
        Instance = this;
        label = GetComponent<TextMeshProUGUI>();
        rectTransform = GetComponent<RectTransform>();

        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        baseScale = rectTransform.localScale;
        HideImmediate();
    }

    public void ShowStompHint()
    {
        if (label == null) return;

        label.text = "STOMP ON HIS HEAD";

        if (showRoutine != null)
        {
            StopCoroutine(showRoutine);
        }

        showRoutine = StartCoroutine(ShowRoutine());
    }

    public void Hide()
    {
        if (showRoutine != null)
        {
            StopCoroutine(showRoutine);
            showRoutine = null;
        }

        HideImmediate();
    }

    private void HideImmediate()
    {
        gameObject.SetActive(false);
        canvasGroup.alpha = 0f;
        rectTransform.localScale = baseScale;
    }

    private IEnumerator ShowRoutine()
    {
        gameObject.SetActive(true);
        canvasGroup.alpha = 0f;
        rectTransform.localScale = baseScale * pulseScaleMin;

        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / fadeDuration);
            canvasGroup.alpha = t;
            rectTransform.localScale = baseScale * Mathf.Lerp(pulseScaleMin, 1f, t);
            yield return null;
        }

        canvasGroup.alpha = 1f;
        rectTransform.localScale = baseScale;

        float pulseTime = 0f;
        while (true)
        {
            pulseTime += Time.deltaTime;
            float pulse = (Mathf.Sin((pulseTime / pulseDuration) * Mathf.PI * 2f) + 1f) * 0.5f;
            rectTransform.localScale = baseScale * Mathf.Lerp(pulseScaleMin, pulseScaleMax, pulse);
            canvasGroup.alpha = Mathf.Lerp(pulseAlphaMin, 1f, pulse);
            yield return null;
        }
    }
}
