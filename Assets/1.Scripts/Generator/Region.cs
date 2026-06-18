using System.Collections.Generic;
using UnityEngine;

public class Region
{
    public List<Vector3> Cells = new();
    public int Count => Cells.Count;

}