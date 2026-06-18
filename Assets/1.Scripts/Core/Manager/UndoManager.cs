using System.Collections.Generic;

public class UndoManager
{
    private Stack<ICommand> history = new();

    public void Execute(ICommand command)
    {
        command.Execute();

        history.Push(command);
    }


    public void Undo()
    {
        if (history.Count == 0)
            return;

        ICommand command = history.Pop();

        command.Undo();
    }

    public void Clear()
    {
        history.Clear();
    }
}