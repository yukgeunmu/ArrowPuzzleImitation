using UnityEngine;

public static class SolverStateFactory
{
    public static SolverState Create(StageDataSO stageData)
    {
        SolverState state = new();

        state.Width = stageData.Width;
        state.Height = stageData.Height;

        foreach (var block in stageData.Blocks)
        {
            switch (block.Type)
            {
                case BlockType.Arrow:

                    SolverArrow arrow = new();

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
}