using UnityEngine;

public class SolverTest : MonoBehaviour
{
    [SerializeField]
    private StageDataSO stageData;

    private void Start()
    {
        SolverState state =
            SolverStateFactory.Create(
                stageData);

        Debug.Log(
            $"Arrow Count : {state.Arrows.Count}");

        Debug.Log(
            $"Obstacle Count : {state.Obstacles.Count}");

        SolverState clone = state.Clone();

        SolverResult result =PuzzleSolver.Solve(state);

        Debug.Log(
            $"Can Solve : {result.CanSolve}");

        Debug.Log(
            $"Min Moves : {result.MinMoves}");

        Debug.Log(result.Difficulty);

    }
}