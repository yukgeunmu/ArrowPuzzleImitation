using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour
{

    public static StageManager instance;

    [SerializeField]
    private CameraController cameraController;

    [SerializeField]
    private StageDataSO stageData;

    [SerializeField]
    private ArrowBlock arrowPrefab;

    [SerializeField]
    private ObstacleBlock obstaclePrefab;

    [SerializeField]
    private GridManager gridManager;

    public List<ArrowBlock> ArrowBlocks = new();


    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        LoadStage();
    }

    private void LoadStage()
    {

        gridManager.Width = stageData.Width;
        gridManager.Height = stageData.Height;

        gridManager.Initialize(stageData.Width, stageData.Height);

        foreach (var info in stageData.Blocks)
        {
            switch (info.Type)
            {
                case BlockType.Arrow:
                    ArrowBlock arrow = Instantiate(arrowPrefab, gridManager.GridToWorld(info.Position), Quaternion.identity);

                    arrow.Init(info.Cells, info.HeadDirection ,gridManager);

                    ArrowBlocks.Add(arrow);

                    gridManager.RegisterBlock(arrow);

                    break;
                case BlockType.Obstacle:

                    ObstacleBlock obstacle = Instantiate(obstaclePrefab, gridManager.GridToWorld(info.Position), Quaternion.identity);

                    obstacle.GridPos = info.Position;

                    gridManager.RegisterBlock(obstacle);

                    break;
            }

        }

        cameraController.FitToGrid(stageData.Width, stageData.Height);

    }

}