using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class ClearPopupUI : MonoBehaviour
{
    [SerializeField]
    private GameObject root;

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

        nextButton.onClick.AddListener(
            StageManager.instance.NextStage);

        retryButton.onClick.AddListener(
            StageManager.instance.RetryStage);
    }

    public void Show()
    {
        root.SetActive(true);

        root.transform.localScale = Vector3.zero;

        root.transform
            .DOScale(1f, 0.25f)
            .SetEase(Ease.OutBack);
    }

    public void Hide()
    {
        root.SetActive(false);
    }
}