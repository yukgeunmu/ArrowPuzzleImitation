using UnityEngine;

public class UndoButton : MonoBehaviour
{
    public void OnClickUndo()
    {
        UndoManager.Instance.Undo();
    }
}