using System.Collections.Generic;
using UnityEngine;

public static class SolverUtility
{
    public static bool IsOutOfGrid( SolverState state, Vector3 pos)
    {
        return pos.x < 0 ||
               pos.y < 0 ||
               pos.x >= state.Width ||
               pos.y >= state.Height;
    }

    public static bool IsOccupied(SolverState state, Vector3 pos)
    {
        if (state.Obstacles.Contains(pos))
            return true;

        foreach (var arrow in state.Arrows)
        {
            if (arrow.Cells.Contains(pos))
                return true;
        }

        return false;
    }

    public static bool CanMoveArrow( SolverState state, SolverArrow arrow)
    {
        Vector3 nextHead =
            arrow.HeadCell +
            arrow.HeadDirection.ToVector();

        // 밖이면 탈출 가능
        if (IsOutOfGrid(state, nextHead))
            return true;

        if (state.Obstacles.Contains(nextHead))
            return false;

        foreach (var otherArrow in state.Arrows)
        {
            if (otherArrow.Cells.Contains(nextHead))
                return false;
        }

        return true;
    }

    public static SolverState MoveArrow(SolverState state, int arrowIndex)
    {
        SolverState nextState = state.Clone();

        SolverArrow arrow = nextState.Arrows[arrowIndex];

        MoveArrowUntilStop(nextState, arrow);

        return nextState;
    }

    private static void MoveArrowUntilStop(SolverState state, SolverArrow arrow)
    {
        int safety = 0;

        while (true)
        {

            if (IsArrowExited(state, arrow))
            {
                state.Arrows.Remove(arrow);
                return;
            }

            if (!CanMoveArrow(state, arrow))
            {
                return;
            }

            MoveOneStep(state, arrow);

            safety++;

            if (safety > 100)
            {
                Debug.LogError(
                    "Infinite Loop");

                return;
            }

        }


    }

    private static void MoveOneStep(SolverState state, SolverArrow arrow)
    {
        Vector3 nextHead =
            arrow.HeadCell +
            arrow.HeadDirection.ToVector();

        arrow.Cells.RemoveAt(0);

        arrow.Cells.Add(nextHead);
    }

    private static bool IsArrowExited(SolverState state, SolverArrow arrow)
    {
        foreach (var cell in arrow.Cells)
        {
            if (!IsOutOfGrid(state, cell))
                return false;
        }

        return true;
    }
}