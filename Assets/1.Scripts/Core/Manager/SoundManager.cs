using UnityEngine;


public enum SFXType
{
    Button,
    Move,
    Blocked,
    Undo,
    Clear
}

public class SoundManager
{

    private AudioSource sfxSource;

    private GameObject soundRoot;


    public void Init()
    {
        soundRoot = new GameObject("SoundManager");

        soundRoot.transform.SetParent(Manager.Instance.transform);

        sfxSource = soundRoot.AddComponent<AudioSource>();

        sfxSource.playOnAwake = false;

    }

    public void Play(SFXType type)
    {
        var clip = Manager.Instance.Resource.GetData<AudioClip>("Sound", type.ToString());

        sfxSource.PlayOneShot(clip);
    }
}
