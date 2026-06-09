using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class StageManager
{
    private int currentStageIndex;

    public int StageLength;

    public List<ArrowBlock> ArrowBlocks = new();

    public event Action<int, int> OnStageLoaded;

    public void Init()
    {
        currentStageIndex = SaveManager.CurrentStage;

        LoadStage();

        Manager.Instance.Grid.OnAllBlocksRemoved += HandleStageClear;
    }

    private void LoadStage()
    {
        int nextArrowId = 0;

        StageLength = Manager.Instance.Resource.GetTableData<StageDataSO>("Stage").Count;

        StageDataSO stage = Manager.Instance.Resource.GetData<StageDataSO>("Stage", $"Stage{currentStageIndex + 1}");

        Manager.Instance.UI.GetScene<HUDUI>().SetStage(currentStageIndex + 1);

        Manager.Instance.Grid.Init(stage.Width, stage.Height);

        foreach (var info in stage.Blocks)
        {
            switch (info.Type)
            {
                case BlockType.Arrow:

                    ArrowBlock arrow = Manager.Instance.Pool.Get();

                    arrow.transform.SetPositionAndRotation(Manager.Instance.Grid.GridToWorld(info.Position), Quaternion.identity);

                    arrow.Init(info.Cells, info.HeadDirection , nextArrowId++);

                    ArrowBlocks.Add(arrow);

                    Manager.Instance.Grid.RegisterBlock(arrow);

                    break;
            }

        }

        OnStageLoaded?.Invoke(stage.Width, stage.Height);

    }

    private void HandleStageClear()
    {
        _ = Manager.Instance.UI.ShowPopup<ClearPopupUI>();
        Manager.Instance.Sound.Play(SFXType.Clear);
    }

    public void NextStage()
    {
        Manager.Instance.UI.ClosePopup();

        currentStageIndex++;

        SaveManager.CurrentStage = currentStageIndex;

        if (currentStageIndex >= Manager.Instance.Resource.GetTableData<StageDataSO>("Stage").Count)
        {
            currentStageIndex = Manager.Instance.Resource.GetTableData<StageDataSO>("Stage").Count - 1;

            Debug.Log("All Clear");

            return;
        }

        ClearCurrentStage();

        LoadStage();
    }

    private void ClearCurrentStage()
    {
        foreach (var arrow in ArrowBlocks)
        {
            if (arrow != null)
            {
                arrow.gameObject.SetActive(false);
            }
        }


        ArrowBlocks.Clear();

        Manager.Instance.Undo.Clear();
    }

    public void RetryStage()
    {
        Manager.Instance.UI.ClosePopup();

        ClearCurrentStage();

        LoadStage();
    }


    public void ResetProgress()
    {
        SaveManager.Clear();

        currentStageIndex = 0;

        RetryStage();
    }


    public void LoadStage(int stageIndex)
    {
        Manager.Instance.Undo.Clear();

        currentStageIndex = stageIndex;

        ClearCurrentStage();

        LoadStage();
    }


    public ArrowBlock GetArrowById( int id)
    {
        return ArrowBlocks.Find( x => x.Id == id);
    }
}