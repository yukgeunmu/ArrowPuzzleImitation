using UnityEngine;

public class StageManager : MonoBehaviour
{
    [SerializeField]
    private StageDataSO stageData;

    [SerializeField]
    private ArrowBlock blockPrefab;

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
            ArrowBlock block =  Instantiate(blockPrefab, GridToWorld(info.Position), Quaternion.identity);

            block.Init( info.Position, info.Direction, gridManager);

            gridManager.RegisterBlock( info.Position, block);
        }
    }

    private Vector3 GridToWorld(Vector2Int gridPos)
    {
        return new Vector3( gridPos.x, gridPos.y, 0);
    }
}