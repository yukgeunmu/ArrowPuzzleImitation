using System.Collections.Generic;
using UnityEngine;

public class MoveCommand : ICommand
{
    private ArrowBlock block;

    private List<Vector3> previousCells;

    private Direction previousDirection;




    public MoveCommand(ArrowBlock block)
    {
        this.block = block;

        previousCells = new List<Vector3>(block.Cells);
        previousDirection = block.HeadDirection;
    }

    public void Execute()
    {
        block.StartMove();
    }

    public void Undo()
    {
        block.Restore(previousCells, previousDirection);
    }
}