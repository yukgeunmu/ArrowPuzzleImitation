using UnityEngine;
using UnityEngine.UI;

public class LevelSelectUI : SceneUI
{

    [SerializeField]
    private Button easyButton;

    [SerializeField]
    private Button normalButton;

    [SerializeField]
    private Button hardButton;

    [SerializeField]
    private Button expertButton;

    [SerializeField]
    private Button masterButton;

    [SerializeField]
    private Button exitButton;

    private void Awake()
    {
        Init();
    }

    public void Init()
    {
        Open();

        easyButton.onClick.AddListener(() => OnClickStartGame(6, 6, 6, Difficulty.Easy));

        normalButton.onClick.AddListener(() => OnClickStartGame(8, 8, 8, Difficulty.Normal));

        hardButton.onClick.AddListener(() => OnClickStartGame(10, 10, 10, Difficulty.Hard));

        expertButton.onClick.AddListener(() => OnClickStartGame(15, 15, 15, Difficulty.Expert));

        masterButton.onClick.AddListener(() => OnClickStartGame(20, 20, 20, Difficulty.Master));

        exitButton.onClick.AddListener(OnClickExit);
    }

    private void OnClickStartGame(int width, int height, int maxLen, Difficulty difficulty)
    {
        Manager.Instance.Sound.Play(SFXType.Button);

        Manager.Instance.StartGame(width, height, maxLen, difficulty);
 
    }

    private void OnClickExit()
    {
        OnClickAnimation(exitButton);

        Manager.Instance.ExitGame();
    }

}
