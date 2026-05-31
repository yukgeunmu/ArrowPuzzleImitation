using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Stage/StageData")]
public class StageDataSO : ScriptableObject
{
    public int Width = 5;
    public int Height = 5;

    public List<BlockInfo> Blocks = new();
}