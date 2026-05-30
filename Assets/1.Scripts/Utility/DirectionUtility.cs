using UnityEngine;

public static class DirectionUtility
{
    public static Vector2Int ToVector(this Direction dir)
    {
        return dir switch
        {
            Direction.Up => Vector2Int.up,
            Direction.Down => Vector2Int.down,
            Direction.Left => Vector2Int.left,
            Direction.Right => Vector2Int.right,
            _ => Vector2Int.zero
        };

    }


}