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

    public readonly InputManager Input = new InputManager();

    public readonly UndoManager Undo = new UndoManager();

    public readonly HintManager Hint = new HintManager();

    public readonly PoolManager Pool = new PoolManager();


    private void Awake()
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

        Input.Init();

    }

    private async void Start()
    {
        await Resource.LoadDataAsync<StageDataSO>("Stage");
        await Resource.LoadDataAsync<AudioClip>("Sound");
        await Resource.LoadDataAsync<GameObject>("UI");
        await Resource.LoadAsync<GameObject>("Arrow");

        Sound.Init();
        UI.Init();

        UI.SetSceneUI<HUDUI>();

        Stage.Init();
    }

    private void OnEnable()
    {
        Input.OnEnable();
    }

    private void OnDisable()
    {
        Input.OnDisable();
    }


}
