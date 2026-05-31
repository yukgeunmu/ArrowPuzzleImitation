using UnityEngine;

public class StageManager : MonoBehaviour
{
    [SerializeField]
    private StageDataSO stageData;

    [SerializeField]
    private ArrowBlock arrowPrefab;

    [SerializeField]
    private ObstacleBlock obstaclePrefab;

    [SerializeField]
    private GridManager gridManager;

    private void Start()
    {
        LoadStage();
    }

    private void LoadStage()
    {
        gridManager.Width = stageData.Width;
        gridManager.Height = stageData.Height;

        foreach (var info in stageData.Blocks)
        {
            BlockBase block = null;

            switch (info.Type)
            {
                case BlockType.Arrow:
                    block = Instantiate(arrowPrefab, gridManager.GridToWorld(info.Position), Quaternion.identity);

                    ((ArrowBlock)block).Init(info.Position, info.Direction, gridManager);

                    break;
                case BlockType.Obstacle:

                    block = Instantiate(obstaclePrefab, gridManager.GridToWorld(info.Position), Quaternion.identity);

                    block.GridPos = info.Position;

                    break;
            }

            gridManager.RegisterBlock(info.Position, block);
        }

        
    }

}