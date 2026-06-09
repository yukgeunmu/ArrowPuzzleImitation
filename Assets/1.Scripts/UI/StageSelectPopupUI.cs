using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class StageSelectPopupUI : PopupUI
{
    [SerializeField]
    private Transform content;

    [SerializeField]
    private StageButtonUI stageButtonPrefab;

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
            StageButtonUI button =
                Instantiate(
                    stageButtonPrefab,
                    content);

            button.Init(i, this);
            buttons.Add(button);
        }
    }

    public override void Open()
    {
        base.Open();
        UnlockStage();
    }

    public override void Close()
    {
        base.Close();

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
}