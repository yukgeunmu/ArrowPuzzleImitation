using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUDUI : MonoBehaviour
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
        SoundManager.Instance.Play(SFXType.Undo);
        UndoManager.Instance.Undo();
    }

    private void OnRestart()
    {
        SoundManager.Instance.Play(SFXType.Button);
        StageManager.instance.RetryStage();
    }

    private void OnReset()
    {
        SoundManager.Instance.Play(SFXType.Button);
        StageManager.instance.ResetProgress();
    }


    private void OpenStageSelect()
    {
        SoundManager.Instance.Play(SFXType.Button);
        StageManager.instance.ShowStageSelect();
    }

    private void OnHint()
    {
        HintManager.instance.ShowHint();
    }


}