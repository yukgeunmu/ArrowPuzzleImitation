using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ArrowEditorData
{
    public List<Vector3> Cells = new();
    public Direction HeadDirection = Direction.Right;
}

public class ArrowData
{
    public List<Vector3> Cells = new();

    public Direction HeadDirection;

    public Color Color;
}

public enum EditorActionType
{
    CreateArrow,
    DeleteArrow
}

public class EditorAction
{
    public EditorActionType Type;

    public ArrowData Arrow;
}