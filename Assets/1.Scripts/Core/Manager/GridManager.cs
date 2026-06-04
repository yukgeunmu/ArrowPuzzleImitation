using System;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public int Width;
    public int Height;
    public int CellSize;

    private GridNode[,] nodes;

    public event Action OnAllBlocksRemoved;


    public void Initialize(int width, int height)
    {
        Width = width;
        Height = height;

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

        StageManager.instance.ArrowBlocks.Remove(block);

        if (StageManager.instance.ArrowBlocks.Count <= 0)
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
        float offsetX = -(Width - 1) * CellSize * 0.5f;
        float offsetY = -(Height - 1) * CellSize * 0.5f;

        return new Vector3(
            gridPos.x * CellSize + offsetX,
            gridPos.y * CellSize + offsetY,
            0f);
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