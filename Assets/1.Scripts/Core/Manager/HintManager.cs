using UnityEngine;

public class HintManager
{
    public void ShowHint()
    {
        SolverState state = SolverStateFactory.CreateFromStageManager(Manager.Instance.Stage);

        SolverResult result =
            PuzzleSolver.Solve(state);

        if (!result.CanSolve)
            return;

        if (result.Path.Count == 0)
            return;

        int arrowId =
            result.Path[0];

        ArrowBlock arrow = Manager.Instance.Stage.GetArrowById(arrowId);

        arrow.PlayHint();
    }
}
