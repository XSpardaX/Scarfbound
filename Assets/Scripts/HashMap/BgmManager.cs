using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BgmManager : MonoBehaviour
{
    public static BgmManager Instance { get; private set; }

    private const string MainMenuSceneName = "Main Menu";

    private readonly Dictionary<string, AudioClip> musicById = new Dictionary<string, AudioClip>();
    private AudioSource bgmAudioSource;
    private bool hasFadedInMenuTrack;
    private bool hasFadedInGameplayTrack;
    private Coroutine fadeRoutine;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Bootstrap()
    {
        if (Instance != null)
        {
            return;
        }

        GameObject bgmManagerObject = new GameObject("BgmManager");
        bgmManagerObject.AddComponent<BgmManager>();
        DontDestroyOnLoad(bgmManagerObject);
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        Initialize();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Start()
    {
        PlayForScene(SceneManager.GetActiveScene().name);
    }

    private void Initialize()
    {
        if (bgmAudioSource != null)
        {
            return;
        }

        bgmAudioSource = gameObject.AddComponent<AudioSource>();
        bgmAudioSource.loop = true;
        bgmAudioSource.playOnAwake = false;
        bgmAudioSource.spatialBlend = 0f;
        bgmAudioSource.volume = 1f;

        LoadMusicClips();
    }

    private void LoadMusicClips()
    {
        BgmLibrary library = Resources.Load<BgmLibrary>("BgmLibrary");
        if (library != null)
        {
            library.RegisterAll(this);
        }

        if (!musicById.ContainsKey(BgmIds.Menu))
        {
            Register(BgmIds.Menu, Resources.Load<AudioClip>("MenuMusic"));
        }

        if (!musicById.ContainsKey(BgmIds.Gameplay))
        {
            Register(BgmIds.Gameplay, Resources.Load<AudioClip>("GameplayMusic"));
        }

#if UNITY_EDITOR
        if (!musicById.ContainsKey(BgmIds.Menu))
        {
            Register(BgmIds.Menu, UnityEditor.AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/SFX/Menu.mp3"));
        }

        if (!musicById.ContainsKey(BgmIds.Gameplay))
        {
            Register(BgmIds.Gameplay, UnityEditor.AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/SFX/Gameplay.mp3"));
        }
#endif

        if (!musicById.ContainsKey(BgmIds.Menu))
        {
            Debug.LogWarning("BgmManager: Menu music not found. Assign it in Resources/BgmLibrary.asset or Resources/MenuMusic.");
        }

        if (!musicById.ContainsKey(BgmIds.Gameplay))
        {
            Debug.LogWarning("BgmManager: Gameplay music not found. Assign it in Resources/BgmLibrary.asset or Resources/GameplayMusic.");
        }
    }

    public void Register(string id, AudioClip clip)
    {
        if (string.IsNullOrEmpty(id))
        {
            return;
        }

        if (clip == null)
        {
            return;
        }

        musicById[id] = clip;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        PlayForScene(scene.name);
    }

    private void PlayForScene(string sceneName)
    {
        if (bgmAudioSource == null)
        {
            Initialize();
        }

        bool isMainMenuScene = sceneName == MainMenuSceneName;
        string targetMusicId = isMainMenuScene ? BgmIds.Menu : BgmIds.Gameplay;

        if (!musicById.TryGetValue(targetMusicId, out AudioClip targetMusicClip))
        {
            return;
        }

        if (targetMusicClip == null)
        {
            return;
        }

        if (bgmAudioSource.clip == targetMusicClip && bgmAudioSource.isPlaying)
        {
            return;
        }

        if (fadeRoutine != null)
        {
            StopCoroutine(fadeRoutine);
            fadeRoutine = null;
        }

        bgmAudioSource.clip = targetMusicClip;
        bgmAudioSource.loop = true;

        bool shouldFadeInTrack;
        if (isMainMenuScene)
        {
            shouldFadeInTrack = !hasFadedInMenuTrack;
            hasFadedInMenuTrack = true;
        }
        else
        {
            shouldFadeInTrack = !hasFadedInGameplayTrack;
            hasFadedInGameplayTrack = true;
        }

        if (shouldFadeInTrack)
        {
            fadeRoutine = StartCoroutine(FadeInAndPlay(1f, 1f));
        }
        else
        {
            bgmAudioSource.volume = 1f;
            bgmAudioSource.Play();
        }
    }

    private IEnumerator FadeInAndPlay(float targetVolume, float fadeDuration)
    {
        float duration = Mathf.Max(0.01f, fadeDuration);
        float elapsed = 0f;

        bgmAudioSource.volume = 0f;
        bgmAudioSource.Play();

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float normalizedTime = Mathf.Clamp01(elapsed / duration);
            bgmAudioSource.volume = Mathf.Lerp(0f, targetVolume, normalizedTime);
            yield return null;
        }

        bgmAudioSource.volume = targetVolume;
        fadeRoutine = null;
    }
}
