using System.Collections.Generic;

public class SolverResult
{
    public bool CanSolve;

    public int MinMoves;

    public List<int> Path = new();

    // 실패 디버깅용
    public int MaxSolvedArrows;

    public int RemainArrows;

    public SolverState BestState;

    public int VisitedCount;

    public string Difficulty
    {
        get
        {
            if (!CanSolve)
                return "Impossible";

            if (MinMoves <= 10)
                return "Easy";

            if (MinMoves <= 20)
                return "Medium";

            if (MinMoves <= 30)
                return "Hard";

            return "Expert";
        }
    }
}