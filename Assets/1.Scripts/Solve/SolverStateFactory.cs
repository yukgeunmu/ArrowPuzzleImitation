using System.Collections.Generic;
using UnityEngine;

public static class SolverStateFactory
{
    public static SolverState Create(StageDataSO stageData)
    {
        SolverState state = new();

        state.Width = stageData.Width;
        state.Height = stageData.Height;

        int arrowId = 0;


        foreach (var block in stageData.Blocks)
        {
            switch (block.Type)
            {
                case BlockType.Arrow:

                    SolverArrow arrow = new();

                    arrow.Id = arrowId++;

                    arrow.HeadDirection = block.HeadDirection;

                    arrow.Cells = new(block.Cells);

                    state.Arrows.Add(arrow);

                    break;

                case BlockType.Obstacle:

                    state.Obstacles.Add(block.Position);

                    break;
            }
        }

        return state;
    }

    public static SolverState CreateFromStageManager(
    StageManager stageManager)
    {
        SolverState state = new();

        state.Width =
            stageManager.GridManager.Width;

        state.Height =
            stageManager.GridManager.Height;

        foreach (var arrow in stageManager.ArrowBlocks)
        {
            SolverArrow solverArrow = new();

            solverArrow.Id = arrow.Id;

            solverArrow.HeadDirection = arrow.HeadDirection;

            solverArrow.Cells = new List<Vector3>(arrow.Cells);

            state.Arrows.Add(solverArrow);
        }

        foreach (var obstacle in stageManager.ObstacleBlocks)
        {
            state.Obstacles.Add(obstacle.GridPos);
        }

        return state;
    }
}