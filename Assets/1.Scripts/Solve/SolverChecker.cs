public static class SolverChecker
{
    public static bool IsClear(SolverState state)
    {
        return state.Arrows.Count == 0;
    }
}