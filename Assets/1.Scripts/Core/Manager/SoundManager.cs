using UnityEngine;


public enum SFXType
{
    Button,
    Move,
    Blocked,
    Undo,
    Clear
}

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [SerializeField]
    private AudioSource sfxSource;

    [SerializeField]
    private AudioClip buttonClip;

    [SerializeField]
    private AudioClip moveClip;

    [SerializeField]
    private AudioClip blockedClip;

    [SerializeField]
    private AudioClip undoClip;

    [SerializeField]
    private AudioClip clearClip;

    private void Awake()
    {
        Instance = this;
    }

    public void Play(SFXType type)
    {
        switch (type)
        {
            case SFXType.Button:
                sfxSource.PlayOneShot(buttonClip);
                break;

            case SFXType.Move:
                sfxSource.PlayOneShot(moveClip);
                break;

            case SFXType.Blocked:
                sfxSource.PlayOneShot(blockedClip);
                break;

            case SFXType.Undo:
                sfxSource.PlayOneShot(undoClip);
                break;

            case SFXType.Clear:
                sfxSource.PlayOneShot(clearClip);
                break;
        }
    }
}
