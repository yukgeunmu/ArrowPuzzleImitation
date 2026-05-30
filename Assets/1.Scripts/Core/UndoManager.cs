using System.Collections.Generic;
using UnityEngine;

public class UndoManager : MonoBehaviour
{
    public static UndoManager Instance;

    private Stack<MoveRecord> history = new();

    private void Awake()
    {
        Instance = this;
    }

    public void RecordMove(ArrowBlock block)
    {
        history.Push(new MoveRecord
        {
            Block = block,
            GridPos = block.GridPos
        });
    }

    public void Undo()
    {
        if (history.Count == 0)
            return;

        MoveRecord record = history.Pop();

        record.Block.UndoMove(record.GridPos);
    }
}