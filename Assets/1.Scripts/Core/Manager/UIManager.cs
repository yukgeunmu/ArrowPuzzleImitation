using System.Collections.Generic;
using UnityEngine;

public class UIManager
{
    private SceneUI currentSceneUI;
    public SceneUI CurrentSceneUI => currentSceneUI;

    private Stack<PopupUI> popupStack = new();

    private Transform sceneRoot;

    private Transform popupRoot;

    public bool HasPopup => popupStack.Count > 0;

    public void Init()
    {
        BindRoots();

        Manager.Instance.Grid.OnAllBlocksRemoved += HandleStageClear;

    }

    private void BindRoots()
    {
        UIRoot root = Object.FindFirstObjectByType<UIRoot>();


        if (root == null)
        {
            GameObject rootObject = new GameObject("UIRoot");

            rootObject.transform.SetParent(Manager.Instance.transform);

            root = rootObject.AddComponent<UIRoot>();

            CreateCanvasRoots(root);
        }

        sceneRoot = root.SceneRoot;
        popupRoot = root.PopupRoot;

    }


    private void CreateCanvasRoots(UIRoot root)
    {
        GameObject sceneCanvas = CreateCanvas("SceneCanvas", 10);

        GameObject popupCanvas = CreateCanvas("PopupCanvas", 11);

        sceneCanvas.transform.SetParent(root.transform);

        popupCanvas.transform.SetParent(root.transform);

        root.SetRoots(sceneCanvas.transform, popupCanvas.transform);
    }

    private GameObject CreateCanvas(string name, int order)
    {
        GameObject canvasObject = new GameObject(name);

        Canvas canvas = canvasObject.AddComponent<Canvas>();

        canvas.renderMode = RenderMode.ScreenSpaceOverlay;

        canvas.sortingOrder = order;

        canvasObject.AddComponent<UnityEngine.UI.CanvasScaler>();

        canvasObject.AddComponent<UnityEngine.UI.GraphicRaycaster>();

        return canvasObject;
    }

    private void SetSceneUI<T>() where T : SceneUI
    {
        string key = typeof(T).Name;

        GameObject prefab = Manager.Instance.Resource.GetData<GameObject>("UI", key);

        GameObject instance = Object.Instantiate(prefab, sceneRoot);

        currentSceneUI = instance.GetComponent<SceneUI>();

        Manager.Instance.Pool.PushSceneUI<T>(currentSceneUI);
    }

    public T ShowPopup<T>() where T : PopupUI
    {
        string key = typeof(T).Name;

        T popup = Manager.Instance.Pool.GetPopupUI<T>(key);

        if (popup == null)
        {
            GameObject prefab = Manager.Instance.Resource.GetData<GameObject>("UI", key);

            popup = Object.Instantiate(prefab, popupRoot).GetComponent<T>();

            popup.transform.SetParent(popupRoot, false);
        }

        popup.Open();

        popupStack.Push(popup);

        return popup as T;
    }

    public void ClosePopup<T>()
    {
        if (popupStack.Count <= 0)
            return;

        PopupUI popup = popupStack.Pop();

        popup.Close();

        Manager.Instance.Pool.PushPopupUI<T>(popup);
    }


    public T GetScene<T>() where T : SceneUI
    {
        return currentSceneUI as T;
    }

    private void HandleStageClear()
    {
        Manager.Instance.Sound.Play(SFXType.Clear);
        ClearPopupUI popupUI = ShowPopup<ClearPopupUI>();
        Manager.Instance.OnClear(popupUI);
    }

    public void ChangeScene<T>() where T : SceneUI
    {
        if (currentSceneUI != null)
        {
            currentSceneUI.gameObject.SetActive(false);
        }

        T sceneUI = Manager.Instance.Pool.GetSceneUI<T>();

        if (sceneUI != null)
        {
            currentSceneUI = sceneUI;
            currentSceneUI.gameObject.SetActive(true);
            return;
        }

        SetSceneUI<T>();

    }

    public string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60);

        int seconds = Mathf.FloorToInt(time % 60);

        int milliseconds =
            Mathf.FloorToInt((time * 100) % 100);

        return $"{minutes:00}:{seconds:00}.{milliseconds:00}";
    }
}
