using UnityEngine;

public class MoveCommand : ICommand
{
    private ArrowBlock block;

    private Vector3 previousPos;

    public MoveCommand(ArrowBlock block)
    {
        this.block = block;
        previousPos = block.GridPos;
    }

    public void Execute()
    {
        block.ExitGrid();
    }

    public void Undo()
    {
        block.Restore(previousPos);
    }
}