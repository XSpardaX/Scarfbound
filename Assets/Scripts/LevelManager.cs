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

        StartCoroutine(ScreenFader.FadeIn(fadeDuration));
    }
}
