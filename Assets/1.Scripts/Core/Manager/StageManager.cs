using System;
using System.Collections.Generic;
using UnityEngine;

public class StageManager
{

    public int Width { get; private set; }
    public int Height { get; private set; }
    public int MaxLen { get; private set; }
    public Difficulty Difficulty { get; private set; }

    public List<ArrowBlock> ArrowBlocks = new();

    public event Action<int, int> OnStageLoaded;

    private StageData stage = new StageData();

    public void Init(int width, int height, int maxLen, Difficulty difficulty)
    {
        SetPuzzleSize(width, height, maxLen, difficulty);
        LoadStage();
    }

    private void LoadStage()
    {
        int nextArrowId = 0;

        stage = GenerateStage();

        Manager.Instance.Grid.Init(stage.Width, stage.Height);

        foreach (var info in stage.Blocks)
        {
            switch (info.Type)
            {
                case BlockType.Arrow:

                    ArrowBlock arrow = Manager.Instance.Pool.Get();

                    arrow.transform.SetPositionAndRotation(Manager.Instance.Grid.GridToWorld(info.Position), Quaternion.identity);

                    arrow.Init(info.Cells, info.HeadDirection, nextArrowId++, info.Color);

                    ArrowBlocks.Add(arrow);

                    Manager.Instance.Grid.RegisterBlock(arrow);

                    break;
            }

        }

        OnStageLoaded?.Invoke(stage.Width, stage.Height);

    }

    public void NextStage()
    {
        Manager.Instance.UI.ClosePopup<ClearPopupUI>();

        ClearCurrentStage();

        Manager.Instance.SetTime(true);

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


    public void BackLevelSelectStage()
    {
        Manager.Instance.isPlaying = false;
        Manager.Instance.UI.ChangeScene<LevelSelectUI>();
        ClearCurrentStage();
    }


    public void LoadStage(int stageIndex)
    {
        Manager.Instance.Undo.Clear();

        ClearCurrentStage();

        LoadStage();
    }


    public ArrowBlock GetArrowById(int id)
    {
        return ArrowBlocks.Find(x => x.Id == id);
    }


    private StageData GenerateStage()
    {
        StageData stage = new StageData();
        List<ArrowData> arrows = new List<ArrowData>();
        AutoGeneratorReverse autoGeneratorReverse = new AutoGeneratorReverse();

        arrows = autoGeneratorReverse.Generate(Width, Height, 2, MaxLen);

        stage.Width = Width;
        stage.Height = Height;

        stage.Blocks = new();

        foreach (var arrow in arrows)
        {
            BlockInfo data = new();

            data.Type = BlockType.Arrow;

            data.HeadDirection = arrow.HeadDirection;

            data.Cells = new();

            data.Position = Vector3.zero;

            foreach (var cell in arrow.Cells)
            {
                data.Cells.Add(new Vector3(cell.x, cell.y));
            }

            data.Color = arrow.Color;

            stage.Blocks.Add(data);
        }

        return stage;
    }

    public void SetPuzzleSize(int width, int height, int maxLen, Difficulty difficulty)
    {
        Width = width;
        Height = height;
        MaxLen = maxLen;
        Difficulty = difficulty;
    }

}