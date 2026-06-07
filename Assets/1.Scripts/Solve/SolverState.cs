using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class SolverState
{
    public int Width;
    public int Height;

    public List<SolverArrow> Arrows = new();

    public HashSet<Vector3> Obstacles = new();

    public SolverState Clone()
    {
        SolverState clone = new();

        clone.Width = Width;
        clone.Height = Height;

        foreach (var arrow in Arrows)
        {
            clone.Arrows.Add(
                arrow.Clone());
        }

        clone.Obstacles = new HashSet<Vector3>(Obstacles);

        return clone;
    }

    public string GetKey()
    {
        StringBuilder sb = new();

        foreach (var arrow in Arrows)
        {
            sb.Append("[");
            sb.Append((int)arrow.HeadDirection);
            sb.Append("]");

            foreach (var cell in arrow.Cells)
            {
                sb.Append(cell.x);
                sb.Append(",");
                sb.Append(cell.y);
                sb.Append("|");
            }

            sb.Append(";");
        }

        return sb.ToString();
    }
}