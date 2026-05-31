using System;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public int Width;
    public int Height;
    public int CellSize;

    private Dictionary<Vector3, BlockBase> blocks = new();

    public event Action OnAllBlocksRemoved;

    public int BlockCount => blocks.Count;

    public void RegisterBlock(Vector3 pos, BlockBase block)
    {
        blocks[pos] = block;
    }

    public void RemoveBlock(Vector3 pos)
    {

        blocks.Remove(pos);

        if (blocks.Count == 0)
        {
            OnAllBlocksRemoved?.Invoke();
        }
    }

    public bool HasBlock(Vector3 pos)
    {
        return blocks.ContainsKey(pos);
    }

    public BlockBase GetBlock(Vector3 pos)
    {
        blocks.TryGetValue(pos, out var block);
        return block;
    }



    public bool CanMove(Vector3 startPos, Direction dir)
    {
        Vector3 currentPos = startPos;

        while (true)
        {
            currentPos += dir.ToVector();

            if (GetBlock(currentPos) !=  null)
                return false;

            if (currentPos.x < 0 ||
                currentPos.x >= Width ||
                currentPos.y < 0 ||
                currentPos.y >= Height)
            {
                return true;
            }
        }
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

}