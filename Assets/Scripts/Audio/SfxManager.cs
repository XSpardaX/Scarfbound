using System.Collections.Generic;
using UnityEngine;

public class SfxManager : MonoBehaviour
{
    public static SfxManager Instance { get; private set; }

    private readonly Dictionary<string, AudioClip> clips = new Dictionary<string, AudioClip>();
    private AudioSource audioSource;
    private bool useBossAttack2;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Bootstrap()
    {
        if (Instance != null) return;

        GameObject sfxObject = new GameObject("SfxManager");
        sfxObject.AddComponent<SfxManager>();
        DontDestroyOnLoad(sfxObject);
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

    private void Initialize()
    {
        if (audioSource != null) return;

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f;

        SfxLibrary library = Resources.Load<SfxLibrary>("SfxLibrary");
        if (library != null)
        {
            library.RegisterAll(this);
        }

        if (clips.Count == 0)
        {
            Debug.LogWarning("SfxManager: SfxLibrary is missing or has no clips assigned.");
        }
    }

    public void Register(string id, AudioClip clip)
    {
        if (string.IsNullOrEmpty(id) || clip == null) return;
        clips[id] = clip;
    }

    public void Play(string id, float volume = 1f)
    {
        if (audioSource == null) Initialize();
        if (audioSource == null || !clips.TryGetValue(id, out AudioClip clip) || clip == null) return;
        audioSource.PlayOneShot(clip, volume);
    }

    public void PlayBossAttack(float volume = 1f)
    {
        Play(useBossAttack2 ? SfxIds.BossAttack2 : SfxIds.BossAttack, volume);
        useBossAttack2 = !useBossAttack2;
    }

    public void PlayBossHurt(float volume = 1f)
    {
        Play(SfxIds.BossHurt, volume);
        Play(SfxIds.BossHurt2, volume);
    }
}
