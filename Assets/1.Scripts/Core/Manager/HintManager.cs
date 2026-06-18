public class HintManager
{
    public void ShowHint()
    {
        SolverState state = SolverStateFactory.CreateFromStageManager(Manager.Instance.Stage);

        SolverResult result = PuzzleSolver.Solve(state);

        if (!result.CanSolve)
        {
            return;
        }


        if (result.Path.Count == 0)
        {
            return;
        }


        int arrowId =
            result.Path[0];

        ArrowBlock arrow = Manager.Instance.Stage.GetArrowById(arrowId);

        arrow.PlayHint();
    }

    public SolverArrow GetHintArrow()
    {
        SolverState state = SolverStateFactory.CreateFromStageManager(Manager.Instance.Stage);

        for (int i = 0; i < state.Arrows.Count; i++)
        {
            if (SolverUtility.CanExitArrow(state, i))
                return state.Arrows[i];
        }

        return null;
    }
}
