using System.Collections.Generic;
using UnityEngine;

public static class PuzzleSolver
{
    public static SolverResult Solve(SolverState startState)
    {
        Queue<SolverNode> queue = new();

        HashSet<string> visited = new();

        queue.Enqueue(new SolverNode(startState, 0, null, -1));

        visited.Add(startState.GetKey());

        while (queue.Count > 0)
        {
            SolverNode current = queue.Dequeue();

            if (SolverChecker.IsClear(current.State))
            {
                return BuildResult(current);
            }

            ExpandNode(current, queue,visited);
        }

        return new SolverResult
        {
            CanSolve = false,
            MinMoves = -1
        };
    }

    private static void ExpandNode( SolverNode current, Queue<SolverNode> queue, HashSet<string> visited)
    {
        SolverState state = current.State;

        for (int i = 0; i < state.Arrows.Count; i++)
        {

            if (!SolverUtility.CanMoveArrow( state, state.Arrows[i]))
            {
                continue;
            }


            SolverState nextState = SolverUtility.MoveArrow(state, i);

            string key = nextState.GetKey();

            if (visited.Contains(key))
            {
                continue;
            }


            visited.Add(key);

            queue.Enqueue( new SolverNode(nextState, current.Depth + 1, current, state.Arrows[i].Id));
        }
    }

    private static SolverResult BuildResult(SolverNode goalNode)
    {
        List<int> path = new();

        SolverNode current = goalNode;

        while (current.Parent != null)
        {
            path.Add(current.SelectedArrowId);

            current = current.Parent;
        }

        path.Reverse();

        return new SolverResult
        {
            CanSolve = true,
            MinMoves = goalNode.Depth,
            Path = path
        };
    }
}