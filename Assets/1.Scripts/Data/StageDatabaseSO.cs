using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(
    fileName = "StageDatabase",
    menuName = "Stage/StageDatabase")]
public class StageDatabaseSO : ScriptableObject
{
    public List<StageDataSO> Stages = new();
}