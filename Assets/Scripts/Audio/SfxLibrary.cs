using UnityEngine;

[CreateAssetMenu(fileName = "SfxLibrary", menuName = "Audio/Sfx Library")]
public class SfxLibrary : ScriptableObject
{
    public AudioClip doorUnlock;
    public AudioClip hit;
    public AudioClip keyCollect;
    public AudioClip jump;
    public AudioClip collect;
    public AudioClip bossAttack;
    public AudioClip bossAttack2;
    public AudioClip landing;
    public AudioClip bossHurt;
    public AudioClip bossHurt2;

    public void RegisterAll(SfxManager manager)
    {
        manager.Register(SfxIds.DoorUnlock, doorUnlock);
        manager.Register(SfxIds.Hit, hit);
        manager.Register(SfxIds.KeyCollect, keyCollect);
        manager.Register(SfxIds.Jump, jump);
        manager.Register(SfxIds.Collect, collect);
        manager.Register(SfxIds.BossAttack, bossAttack);
        manager.Register(SfxIds.BossAttack2, bossAttack2);
        manager.Register(SfxIds.Landing, landing);
        manager.Register(SfxIds.BossHurt, bossHurt);
        manager.Register(SfxIds.BossHurt2, bossHurt2);
    }
}
