using System.Collections.Generic;
using UnityEngine;

public class UndoManager : MonoBehaviour
{
    public static UndoManager Instance;

    private Stack<ICommand> history = new();

    private void Awake()
    {
        Instance = this;
    }

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