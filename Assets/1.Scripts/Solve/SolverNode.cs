using UnityEngine;

public class SolverNode
{
    public SolverState State;

    public int Depth;

    public SolverNode(SolverState state, int depth)
    {
        State = state;
        Depth = depth;
    }
}
