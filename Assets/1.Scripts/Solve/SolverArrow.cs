using System.Collections.Generic;
using UnityEngine;

public class SolverArrow
{
    public List<Vector3> Cells = new();

    public Direction HeadDirection;

    public SolverArrow Clone()
    {
        SolverArrow clone = new();

        clone.HeadDirection = HeadDirection;

        clone.Cells = new List<Vector3>(Cells);

        return clone;
    }

    public Vector3 HeadCell
    {
        get
        {
            return Cells[Cells.Count - 1];
        }
    }
}