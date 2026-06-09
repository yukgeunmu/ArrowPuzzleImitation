using UnityEngine;

public class UIRoot : MonoBehaviour
{
    public Transform SceneRoot { get; private set; }

    public Transform PopupRoot { get; private set; }

    public void SetRoots(Transform sceneRoot, Transform popupRoot)
    {
        SceneRoot = sceneRoot;
        PopupRoot = popupRoot;
    }
}
