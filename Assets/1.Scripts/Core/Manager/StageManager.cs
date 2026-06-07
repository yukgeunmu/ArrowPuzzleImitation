using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour
{

    public static StageManager instance;

    [SerializeField]
    private CameraController cameraController;

    [SerializeField]
    private StageDatabaseSO stageDatabase;

    [SerializeField]
    private ClearPopupUI clearPopupUI;

    [SerializeField]
    private HUDUI hUDUI;

    [SerializeField]
    private StageSelectUI stageSelectUI;

    private int currentStageIndex;

    public int CurrentStageIndex => currentStageIndex;

    public StageDataSO CurrentStage
    {
        get
        {
            return stageDatabase.Stages[currentStageIndex];
        }
    }

    [SerializeField]
    private ArrowBlock arrowPrefab;

    [SerializeField]
    private ObstacleBlock obstaclePrefab;

    [SerializeField]
    private GridManager gridManager;


    public GridManager GridManager => gridManager;

    public List<ArrowBlock> ArrowBlocks = new();

    public List<ObstacleBlock> ObstacleBlocks = new();


    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        currentStageIndex = SaveManager.CurrentStage;


        stageSelectUI.Init(stageDatabase.Stages.Count);

        LoadStage();

        gridManager.OnAllBlocksRemoved += HandleStageClear;
    }

    private void LoadStage()
    {
        int nextArrowId = 0;

        hUDUI.SetStage(currentStageIndex + 1);

        gridManager.Width = CurrentStage.Width;
        gridManager.Height = CurrentStage.Height;

        gridManager.Initialize(CurrentStage.Width, CurrentStage.Height);

        foreach (var info in CurrentStage.Blocks)
        {
            switch (info.Type)
            {
                case BlockType.Arrow:
                    //ArrowBlock arrow = Instantiate(arrowPrefab, gridManager.GridToWorld(info.Position), Quaternion.identity);

                    ArrowBlock arrow = ArrowPool.Instance.Get();

                    arrow.transform.SetPositionAndRotation(gridManager.GridToWorld(info.Position), Quaternion.identity);

                    arrow.Init(info.Cells, info.HeadDirection ,gridManager, nextArrowId++);

                    ArrowBlocks.Add(arrow);

                    gridManager.RegisterBlock(arrow);

                    break;
                case BlockType.Obstacle:

                    ObstacleBlock obstacle = Instantiate(obstaclePrefab, gridManager.GridToWorld(info.Position), Quaternion.identity);

                    obstacle.GridPos = info.Position;

                    ObstacleBlocks.Add(obstacle);

                    gridManager.RegisterBlock(obstacle);

                    break;
            }

        }

        cameraController.FitToGrid(CurrentStage.Width, CurrentStage.Height);

    }

    private void HandleStageClear()
    {
        clearPopupUI.Show();
        SoundManager.Instance.Play(SFXType.Clear);
    }

    public void NextStage()
    {
        clearPopupUI.Hide();

        currentStageIndex++;

        SaveManager.CurrentStage = currentStageIndex;

        if (currentStageIndex >= stageDatabase.Stages.Count)
        {
            currentStageIndex = stageDatabase.Stages.Count - 1;

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

        UndoManager.Instance.Clear();
    }

    public void RetryStage()
    {
        clearPopupUI.Hide();

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
        UndoManager.Instance.Clear();

        currentStageIndex = stageIndex;

        ClearCurrentStage();

        LoadStage();
    }

    public void ShowStageSelect()
    {
        stageSelectUI.Show();
    }

    public ArrowBlock GetArrowById( int id)
    {
        return ArrowBlocks.Find( x => x.Id == id);
    }
}