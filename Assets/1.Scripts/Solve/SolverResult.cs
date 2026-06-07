using System.Collections.Generic;

public class SolverResult
{
    public bool CanSolve;

    public int MinMoves;

    public List<int> Path = new();

    public string Difficulty
    {
        get
        {
            if (MinMoves <= 3)
                return "Easy";

            if (MinMoves <= 8)
                return "Medium";

            if (MinMoves <= 15)
                return "Hard";

            return "Expert";
        }
    }
}