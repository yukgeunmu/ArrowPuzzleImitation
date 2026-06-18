using System;
using UnityEngine;

public class GridManager
{
    public int Width;
    public int Height;
    public int CellSize;

    private GridNode[,] nodes;

    public event Action OnAllBlocksRemoved;


    public void Init(int width, int height)
    {
        Width = width;
        Height = height;
        CellSize = 1;

        nodes = new GridNode[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                nodes[x, y] = new GridNode
                {
                    Position = new Vector3(x, y)
                };
            }
        }
    }


    //노드 조회
    public GridNode GetNode(Vector3 pos)
    {
        if (IsOutOfGrid(pos))
            return null;


        return nodes[(int)pos.x, (int)pos.y];
    }


    //블록 등록
    public void RegisterBlock(ArrowBlock block)
    {

        foreach (var cell in block.Cells)
        {
            GridNode node = GetNode(cell);

            if (node == null)
                continue;

            node.OccupiedBlock = block;
        }
    }


    public void RegisterBlock(ObstacleBlock block)
    {
        GridNode node = GetNode(block.GridPos);

        node.OccupiedBlock = block;
    }


    //블록 제거
    public void RemoveBlock(ArrowBlock block)
    {
        foreach (var cell in block.Cells)
        {
            GridNode node = GetNode(cell);

            if (node == null)
                continue;

            node.OccupiedBlock = null;
        }

        Manager.Instance.Stage.ArrowBlocks.Remove(block);

        if (Manager.Instance.Stage.ArrowBlocks.Count <= 0)
        {
            OnAllBlocksRemoved?.Invoke();
        }

    }

    public BlockBase GetBlock(Vector3 pos)
    {
        return GetNode(pos).OccupiedBlock;
    }


    public bool CanMoveShape(ArrowBlock block)
    {
        Vector3 nextPos = block.HeadCell + block.HeadDirection.ToVector();

        if (IsOutOfGrid(nextPos))
            return true;

        GridNode node = GetNode(nextPos);

        if (node == null)
            return true;

        return node.OccupiedBlock == null;
    }


    public bool IsOutOfGrid(Vector3 pos)
    {
        return pos.x < 0 ||
               pos.x >= Width ||
               pos.y < 0 ||
               pos.y >= Height;
    }

    public Vector3 GridToWorld(Vector3 gridPos)
    {
        Vector2 offset = Offset();

        return new Vector3(
            gridPos.x * CellSize - offset.x,
            gridPos.y * CellSize - offset.y,
            0f);
    }

    public Vector2 Offset()
    {
        float offsetX = (Width - 1) * CellSize * 0.5f;
        float offsetY = (Height - 1) * CellSize * 0.5f;

        return new Vector2(offsetX, offsetY);
    }


    public bool IsCompletelyOut(ArrowBlock block)
    {
        foreach (var cell in block.Cells)
        {
            if (!IsOutOfGrid(cell))
                return false;
        }

        return true;
    }

    public void UnregisterBlock(ArrowBlock block)
    {
        foreach (var cell in block.Cells)
        {
            GridNode node = GetNode(cell);

            if (node != null &&
                node.OccupiedBlock == block)
            {
                node.OccupiedBlock = null;
            }
        }
    }

}