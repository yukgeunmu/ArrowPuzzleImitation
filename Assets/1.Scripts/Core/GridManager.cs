using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public int Width;
    public int Height;

    private Dictionary<Vector2Int, ArrowBlock> blocks = new();

    public int BlockCount => blocks.Count;

    public void RegisterBlock(Vector2Int pos, ArrowBlock block)
    {
        blocks[pos] = block;
    }

    public void RemoveBlock(Vector2Int pos)
    {
        if (blocks.ContainsKey(pos))
            blocks.Remove(pos);
    }

    public bool HasBlock(Vector2Int pos)
    {
        return blocks.ContainsKey(pos);
    }

    public ArrowBlock GetBlock(Vector2Int pos)
    {
        blocks.TryGetValue(pos, out ArrowBlock block);
        return block;
    }



    public bool CanMove(Vector2Int startPos, Direction dir)
    {
        Vector2Int currentPos = startPos;

        while (true)
        {
            currentPos += dir.ToVector();

            if (HasBlock(currentPos))
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
}