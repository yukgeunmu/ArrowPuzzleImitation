using UnityEngine;

public class GridNode
{
    public Vector3 Position;

    public BlockBase OccupiedBlock;

    public bool IsOccupied => OccupiedBlock != null;
}