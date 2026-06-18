using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUDUI : SceneUI
{

    [SerializeField]
    private TextMeshProUGUI timerText;

    [SerializeField]
    private Button undoButton;

    [SerializeField]
    private Button backButton;

    [SerializeField]
    private Button hintButton;

    [SerializeField]
    private Button exitButton;


    private void Awake()
    {
        Init();
    }

    public void Init()
    {
        Open();

        undoButton.onClick.AddListener(OnUndo);
        backButton.onClick.AddListener(OnRestart);
        hintButton.onClick.AddListener(OnHint);
        exitButton.onClick.AddListener(OnClickExit);

    }

    private void OnUndo()
    {
        OnClickAnimation(undoButton);
        Manager.Instance.Sound.Play(SFXType.Undo);
        Manager.Instance.Undo.Undo();
    }

    private void OnRestart()
    {
        OnClickAnimation(backButton);
        Manager.Instance.Sound.Play(SFXType.Button);
        Manager.Instance.Stage.BackLevelSelectStage();
    }

    private void OnHint()
    {
        OnClickAnimation(hintButton);
        Manager.Instance.Sound.Play(SFXType.Button);
        SolverArrow solverArrow = Manager.Instance.Hint.GetHintArrow();

        if(solverArrow == null)
        {
            Manager.Instance.Hint.ShowHint();
        }
        else
        {
            ArrowBlock arrow = Manager.Instance.Stage.GetArrowById(solverArrow.Id);
            arrow.PlayHint();
        }
    }

    private void OnClickExit()
    {
        OnClickAnimation(exitButton);

        Manager.Instance.ExitGame();
    }

    public void SetTimer(float elapsed)
    {
        timerText.text = Manager.Instance.UI.FormatTime(elapsed);
    }

}