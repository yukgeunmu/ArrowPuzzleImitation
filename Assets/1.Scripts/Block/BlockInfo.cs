using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BlockInfo
{
    public BlockType Type;
    public List<Vector3> Cells = new();

    public Direction HeadDirection;

    public Vector3 Position;

    public Color Color = Color.black;
}