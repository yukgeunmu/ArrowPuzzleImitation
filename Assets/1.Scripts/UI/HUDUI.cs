using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUDUI : SceneUI
{
    [SerializeField]
    private TMP_Text stageText;

    [SerializeField]
    private Button undoButton;

    [SerializeField]
    private Button restartButton;

    [SerializeField]
    private Button resetButton;

    [SerializeField]
    private Button stageSelectButton;

    [SerializeField]
    private Button hintButton;

    private void Awake()
    {
        Init();
    }

    public void Init()
    {
        Open();

        undoButton.onClick.AddListener(OnUndo);
        restartButton.onClick.AddListener(OnRestart);
        resetButton.onClick.AddListener(OnReset);
        stageSelectButton.onClick.AddListener(OpenStageSelect);
        hintButton.onClick.AddListener(OnHint);

    }

    public void SetStage(int stage)
    {
        stageText.text = $"Stage {stage}";
    }

    private void OnUndo()
    {
        OnClickAnimation(undoButton);
        Manager.Instance.Sound.Play(SFXType.Undo);
        Manager.Instance.Undo.Undo();
    }

    private void OnRestart()
    {
        OnClickAnimation(restartButton);
        Manager.Instance.Sound.Play(SFXType.Button);
        Manager.Instance.Stage.RetryStage();
    }

    private void OnReset()
    {
        OnClickAnimation(resetButton);
        Manager.Instance.Sound.Play(SFXType.Button);
        Manager.Instance.Stage.ResetProgress();
    }


    private void OpenStageSelect()
    {
        OnClickAnimation(stageSelectButton);
        Manager.Instance.Sound.Play(SFXType.Button);
        Manager.Instance.UI.ShowPopup<StageSelectPopupUI>();
    }

    private void OnHint()
    {
        OnClickAnimation(hintButton);
        Manager.Instance.Hint.ShowHint();
    }


}