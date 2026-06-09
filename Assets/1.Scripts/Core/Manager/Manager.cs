using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

public class Manager : MonoBehaviour
{
    public static Manager Instance { get; private set; }

    public readonly ResourceManager Resource = new ResourceManager();

    public readonly SoundManager Sound = new SoundManager();
    
    public readonly StageManager Stage = new StageManager();

    public readonly GridManager Grid = new GridManager();

    public readonly UIManager UI = new UIManager();

    public readonly UndoManager Undo = new UndoManager();

    public readonly HintManager Hint = new HintManager();

    public readonly PoolManager Pool = new PoolManager();


    private async void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        else
        {
            Instance = this;

            DontDestroyOnLoad(gameObject);
        }

        await Resource.LoadDataAsync<StageDataSO>("Stage");
        await Resource.LoadDataAsync<AudioClip>("Sound");
        await Resource.LoadAsync<GameObject>("Arrow");

        UI.Init();
        Sound.Init();

        await UI.SetSceneUI<HUDUI>();

        Stage.Init();

    }


}
