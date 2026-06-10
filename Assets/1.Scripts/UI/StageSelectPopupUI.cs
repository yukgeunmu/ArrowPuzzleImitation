using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageSelectPopupUI : PopupUI
{
    [SerializeField]
    private Transform content;

    //[SerializeField]
    //private StageButtonUI stageButtonPrefab;

    [SerializeField]
    private Button closeButton;

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

            StageButtonUI button = prefab.GetComponent<StageButtonUI>();

            button.Init(i, this);
            buttons.Add(button);

            Instantiate(button, content);
        }

        closeButton.onClick.AddListener(OnClickCloseButton);
    }

    public override void Open()
    {
        base.Open();
        UnlockStage();
    }


    public void SelectStage(int stageIndex)
    {
        Close();

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