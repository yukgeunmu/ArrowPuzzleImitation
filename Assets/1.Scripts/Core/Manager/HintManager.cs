using UnityEngine;

public class HintManager : MonoBehaviour
{
    public static HintManager instance;

    private void Awake()
    {
        instance = this;
    }

    public void ShowHint()
    {
        SolverState state = SolverStateFactory.CreateFromStageManager(StageManager.instance);

        SolverResult result =
            PuzzleSolver.Solve(state);

        if (!result.CanSolve)
            return;

        if (result.Path.Count == 0)
            return;

        int arrowId =
            result.Path[0];

        ArrowBlock arrow = StageManager.instance.GetArrowById(arrowId);

        arrow.PlayHint();
    }
}
