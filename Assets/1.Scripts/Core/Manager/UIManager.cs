using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor.EditorTools;
using UnityEngine;

public class UIManager
{
    private SceneUI currentSceneUI;
    public SceneUI CurrentSceneUI => currentSceneUI;

    private Stack<PopupUI> popupStack = new();

    private Transform sceneRoot;

    private Transform popupRoot;

    public void Init()
    {
        BindRoots();

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

    public async Task SetSceneUI<T>() where T : SceneUI
    {
        string key = typeof(T).Name;

        GameObject prefab = await Manager.Instance.Resource.LoadAsync<GameObject>(key);

        GameObject instance = Object.Instantiate(prefab, sceneRoot);

        currentSceneUI = instance.GetComponent<SceneUI>();
    }

    public async Task<T> ShowPopup<T>() where T : PopupUI
    {
        string key = typeof(T).Name;

        GameObject prefab = await Manager.Instance.Resource.LoadAsync<GameObject>(key);

        T popup = Object.Instantiate(prefab, popupRoot).GetComponent<T>();

        popup.Open();

        popupStack.Push(popup);

        return popup;
    }

    public void ClosePopup()
    {
        if (popupStack.Count <= 0)
            return;

        PopupUI popup = popupStack.Pop();

        popup.Close();
    }


    public T GetScene<T>() where T : SceneUI
    {
        if (currentSceneUI == null)
            Debug.Log("11111111");

        return currentSceneUI as T;
    }
}
