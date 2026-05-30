using UnityEngine;

public class ArrowBlock : MonoBehaviour
{
    public Vector2Int GridPos;
    public Direction Direction;

    private GridManager gridManager;

    public void Init( Vector2Int pos, Direction dir, GridManager manager)
    {
        GridPos = pos;
        Direction = dir;
        gridManager = manager;
    }

    public void TryMove()
    {
        Debug.Log("Click");
        if (!gridManager.CanMove(GridPos, Direction))
            return;

        gridManager.RemoveBlock(GridPos);

        Destroy(gameObject);

        GameManager.Instance.CheckClear();
    }
}