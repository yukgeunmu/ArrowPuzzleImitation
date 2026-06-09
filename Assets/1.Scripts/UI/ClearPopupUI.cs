using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class ClearPopupUI : PopupUI
{
    [SerializeField]
    private Button nextButton;

    [SerializeField]
    private Button retryButton;

    private void Awake()
    {
        Init();
    }

    public void Init()
    {
        Open();

        nextButton.onClick.AddListener(OnClickNextButton);

        retryButton.onClick.AddListener(OnClickRetryButton);
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
        OnClickAnimation();
        Manager.Instance.Stage.NextStage();
    }

    public void OnClickRetryButton()
    {
        OnClickAnimation();
        Manager.Instance.Stage.RetryStage();
    }
}