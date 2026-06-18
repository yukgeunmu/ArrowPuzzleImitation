using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

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

    public readonly SaveManager Save = new SaveManager();


    private float startTime;
    [HideInInspector] public bool isPlaying;

    public Image loadImage;


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
        await Resource.LoadDataAsync<AudioClip>("Sound");
        await Resource.LoadDataAsync<GameObject>("UI");
        await Resource.LoadAsync<GameObject>("Arrow");

        await Task.Delay(500);

        loadImage.gameObject.SetActive(false);

        Sound.Init();
        UI.Init();

        UI.ChangeScene<LevelSelectUI>();
    }

    private void Update()
    {
        if (!isPlaying)
            return;

        float elapsed = Time.time - startTime;

        UI.GetScene<HUDUI>().SetTimer(elapsed);
    }


    private void OnEnable()
    {
        Input.OnEnable();
    }

    private void OnDisable()
    {
        Input.OnDisable();
    }

    public void SetTime(bool isPlay)
    {
        startTime = Time.time;
        isPlaying = isPlay;
    }

    public void StartGame(int width, int height, int maxLen, Difficulty difficulty)
    {
        UI.ChangeScene<HUDUI>();

        Stage.Init(width, height, maxLen, difficulty);

        SetTime(true);
    }

    public void OnClear(ClearPopupUI popupUI)
    {
        isPlaying = false;

        float clearTime = Time.time - startTime;

        Save.SaveBestRecord(Stage.Difficulty, clearTime);

        popupUI.SetTime(Stage.Difficulty, clearTime);
    }

    public void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
    Application.Quit();
#endif
    }
}
