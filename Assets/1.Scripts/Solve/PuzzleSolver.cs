using System.Collections.Generic;

public static class PuzzleSolver
{
    public static SolverResult Solve(SolverState startState)
    {
        Queue<SolverNode> queue = new();

        HashSet<string> visited = new();

        SolverNode startNode = new SolverNode(startState, 0, null, -1);

        queue.Enqueue(startNode);

        visited.Add(startState.GetKey());

        SolverNode bestNode = startNode;

        int bestExitCount = 0;

        while (queue.Count > 0)
        {
            SolverNode current = queue.Dequeue();

            int exitCount = startState.Arrows.Count - current.State.Arrows.Count;

            if (exitCount > bestExitCount)
            {
                bestExitCount = exitCount;
                bestNode = current;
            }

            if (SolverChecker.IsClear(current.State))
            {
                return new SolverResult
                {
                    CanSolve = true,
                    MinMoves = current.Depth,
                    Path = BuildPath(current),

                    MaxSolvedArrows = exitCount,
                    RemainArrows = 0,
                    BestState = current.State
                };
            }

            ExpandNode(current, queue, visited);
        }

        return new SolverResult
        {
            CanSolve = false,
            MinMoves = -1,
            Path = BuildPath(bestNode),

            MaxSolvedArrows = bestExitCount,

            RemainArrows = bestNode.State.Arrows.Count,

            BestState = bestNode.State
        };
    }

    private static void ExpandNode(SolverNode current, Queue<SolverNode> queue, HashSet<string> visited)
    {
        SolverState state = current.State;

        for (int i = 0; i < state.Arrows.Count; i++)
        {

            if (!SolverUtility.CanMoveArrow(state, state.Arrows[i]))
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

            queue.Enqueue(new SolverNode(nextState, current.Depth + 1, current, state.Arrows[i].Id));
        }
    }


    private static List<int> BuildPath(SolverNode node)
    {
        List<int> path = new();

        SolverNode current = node;

        while (current.Parent != null)
        {
            path.Add(current.SelectedArrowId);

            current = current.Parent;
        }

        path.Reverse();

        return path;
    }

    public static SolverResult SolveExitOnly(SolverState startState)
    {
        Queue<SolverNode> queue = new();

        HashSet<string> visited = new();

        SolverNode startNode = new SolverNode(startState, 0, null, -1);

        queue.Enqueue(startNode);

        visited.Add(startState.GetKey());

        SolverNode bestNode = startNode;

        int bestExitCount = 0;

        while (queue.Count > 0)
        {
            SolverNode current = queue.Dequeue();

            int exitCount = startState.Arrows.Count - current.State.Arrows.Count;

            if (exitCount > bestExitCount)
            {
                bestExitCount = exitCount;
                bestNode = current;
            }

            if (SolverChecker.IsClear(current.State))
            {
                return new SolverResult
                {
                    CanSolve = true,
                    MinMoves = current.Depth,
                    Path = BuildPath(current),

                    MaxSolvedArrows = exitCount,
                    RemainArrows = 0,
                    BestState = current.State
                };

            }

            ExpandExitNode(current, queue, visited);
        }

        return new SolverResult
        {
            CanSolve = false,
            MinMoves = -1,
            Path = BuildPath(bestNode),

            MaxSolvedArrows = bestExitCount,
            RemainArrows = bestNode.State.Arrows.Count,
            BestState = bestNode.State
        };
    }

    private static void ExpandExitNode(SolverNode current, Queue<SolverNode> queue, HashSet<string> visited)
    {
        for (int i = 0; i < current.State.Arrows.Count; i++)
        {

            SolverState nextState = SolverUtility.MoveArrow(current.State, i);

            bool exited = nextState.Arrows.Count < current.State.Arrows.Count;

            if (!exited)
                continue;

            string key = nextState.GetKey();

            if (!visited.Add(key))
                continue;

            queue.Enqueue(
                new SolverNode(
                    nextState,
                    current.Depth + 1,
                    current,
                    current.State.Arrows[i].Id));
        }
    }

    public static SolverResult ValidateGeneratedPuzzle(SolverState state)
    {
        int count = state.Arrows.Count;

        while (state.Arrows.Count > 0)
        {
            bool found = false;

            for (int i = 0; i < state.Arrows.Count; i++)
            {
                if (SolverUtility.CanExitArrow(state, i))
                {
                    state = SolverUtility.MoveArrow(state, i);
                    found = true;
                    break;
                }
            }

            if (!found)
            {
                return new SolverResult
                {
                    CanSolve = false,
                    MinMoves = -1
                };
            }
        }

        return new SolverResult
        {
            CanSolve = true,
            MinMoves = count
        };
    }
}