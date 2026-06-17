using System.Collections;
using UnityEngine;

public class EndingTrigger : MonoBehaviour
{
    public EndScreenManager endScreenManager;
    public float fadeOutDuration = 1f;
    public float fadeInDuration = 1f;

    private bool hasTriggered;

    private void OnTriggerEnter(Collider other)
    {
        if (hasTriggered) return;
        if (!other.CompareTag("Player")) return;
        if (CheckpointManager.IsRespawning) return;
        if (EndingState.isInEnding) return;

        PlayerHealth playerHealth = other.GetComponentInParent<PlayerHealth>();
        if (playerHealth != null && playerHealth.IsInvincible) return;

        hasTriggered = true;
        StartCoroutine(PlayEndingSequence());
    }

    private IEnumerator PlayEndingSequence()
    {
        EndingState.isInEnding = true;
        Time.timeScale = 1f;

        if (PauseMenuManager.IsPaused)
        {
            FindFirstObjectByType<PauseMenuManager>()?.Resume();
        }

        CursorController.ApplyUnlocked();

        yield return ScreenFader.FadeOut(fadeOutDuration);

        EndScreenManager manager = ResolveEndScreenManager();
        if (manager != null)
        {
            manager.ShowEndScreen();
        }

        UnityEngine.UI.Image fadeImage = ScreenFader.FadeImage;
        if (fadeImage != null)
        {
            fadeImage.transform.SetAsLastSibling();
        }

        yield return ScreenFader.FadeIn(fadeInDuration);
    }

    private EndScreenManager ResolveEndScreenManager()
    {
        if (endScreenManager != null)
        {
            return endScreenManager;
        }

        return FindFirstObjectByType<EndScreenManager>();
    }
}
