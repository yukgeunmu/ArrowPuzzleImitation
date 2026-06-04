using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ArrowEditorData
{
    public List<Vector3> Cells = new();
    public Direction HeadDirection = Direction.Right;
}