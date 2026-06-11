using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageSelectPopupUI : PopupUI
{
    [SerializeField]
    private Transform content;

    [SerializeField]
    private Button closeButton;

    [SerializeField]
    private Button bgCloseButton;

    private List<StageButtonUI> buttons;

    private void Awake()
    {
        Init();
    }


    public void Init()
    {
        buttons = new List<StageButtonUI>();

        foreach (Transform child in content)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < Manager.Instance.Stage.StageLength; i++)
        {
            GameObject prefab = Manager.Instance.Resource.GetData<GameObject>("UI", typeof(StageButtonUI).Name);

            StageButtonUI button = Instantiate(prefab, content).GetComponent<StageButtonUI>();

            button.Init(i, this);
            buttons.Add(button);
        }

        closeButton.onClick.AddListener(OnClickCloseButton);
        bgCloseButton.onClick.AddListener(OnClickCloseButton);
    }

    public override void Open()
    {
        base.Open();
        UnlockStage();
    }


    public void SelectStage(int stageIndex)
    {
        OnClickCloseButton();
        Manager.Instance.Stage.LoadStage(stageIndex);
    }


    public void UnlockStage()
    {
        foreach (var button in buttons)
        {
            button.Unlock();
        }
    }

    public void OnClickCloseButton()
    {
        Manager.Instance.UI.ClosePopup<StageSelectPopupUI>();
    }
}