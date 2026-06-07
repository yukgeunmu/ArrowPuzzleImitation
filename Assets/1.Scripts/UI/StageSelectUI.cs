using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class StageSelectUI : MonoBehaviour
{
    [SerializeField]
    private GameObject root;

    [SerializeField]
    private Transform content;

    [SerializeField]
    private StageButtonUI stageButtonPrefab;

    private StageManager stageManager;

    private List<StageButtonUI> buttons;


    public void Init(int stageCount)
    {
        stageManager = StageManager.instance;
        buttons = new List<StageButtonUI>();

        foreach (Transform child in content)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < stageCount; i++)
        {
            StageButtonUI button =
                Instantiate(
                    stageButtonPrefab,
                    content);

            button.Init(i, this);
            buttons.Add(button);
        }
    }

    public void SelectStage(int stageIndex)
    {
        Hide();

        stageManager.LoadStage(stageIndex);
    }

    public void Show()
    {
        root.SetActive(true);
        UnlockStage();
    }

    public void Hide()
    {
        root.SetActive(false);
    }

    public void UnlockStage()
    {
        foreach (var button in buttons)
        {
            button.Unlock();
        }
    }
}