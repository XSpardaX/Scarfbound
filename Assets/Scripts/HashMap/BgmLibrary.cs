using UnityEngine;

[CreateAssetMenu(fileName = "BgmLibrary", menuName = "Audio/Bgm Library")]
public class BgmLibrary : ScriptableObject
{
    public AudioClip menuMusic;
    public AudioClip gameplayMusic;

    public void RegisterAll(BgmManager manager)
    {
        manager.Register(BgmIds.Menu, menuMusic);
        manager.Register(BgmIds.Gameplay, gameplayMusic);
    }
}
