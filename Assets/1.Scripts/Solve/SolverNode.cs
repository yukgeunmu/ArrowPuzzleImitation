using UnityEngine;

public class SolverNode
{
    public SolverState State;

    public int Depth;

    public SolverNode Parent;

    public int SelectedArrowId;

    public SolverNode(SolverState state, int depth)
    {
        State = state;
        Depth = depth;
    }

    public SolverNode(SolverState state,int depth, SolverNode parent, int Id)
    {
        State = state;
        Depth = depth;
        Parent = parent;
        SelectedArrowId = Id;
    }
}
