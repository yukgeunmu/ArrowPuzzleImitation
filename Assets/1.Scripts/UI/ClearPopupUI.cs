using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ClearPopupUI : PopupUI
{
    [SerializeField]
    private Button nextButton;

    [SerializeField]
    private TextMeshProUGUI clearTimeText;

    [SerializeField]
    private TextMeshProUGUI bestTimeText;

    private void Awake()
    {
        Init();
    }


    public void Init()
    {
        Open();

        nextButton.onClick.AddListener(OnClickNextButton);
    }


    public override void Open()
    {
        base.Open();

        this.transform.localScale = Vector3.zero;

        this.transform
            .DOScale(1f, 0.25f)
            .SetEase(Ease.OutBack);
    }

    public void OnClickNextButton()
    {
        OnClickAnimation(nextButton);
        Manager.Instance.Stage.NextStage();
    }

    public void SetTime(Difficulty difficulty,float clearTime)
    {
        clearTimeText.text = $"TIME: {Manager.Instance.UI.FormatTime(clearTime)}";

        float bestTime = Manager.Instance.Save.GetBestRecord(difficulty);

        bestTimeText.text = $"BEST: {Manager.Instance.UI.FormatTime(bestTime)}";
    }

}