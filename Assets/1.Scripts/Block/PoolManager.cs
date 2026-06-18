using System.Collections.Generic;
using UnityEngine;

public class PoolManager
{
    private readonly List<ArrowBlock> pool = new();
    private readonly Dictionary<string, PopupUI> popUIPool = new();
    private readonly Dictionary<string, SceneUI> scenePool = new();

    public ArrowBlock Get()
    {
        foreach (var arrow in pool)
        {
            if (!arrow.gameObject.activeSelf)
            {
                arrow.gameObject.SetActive(true);
                return arrow;
            }
        }

        GameObject prefab = Manager.Instance.Resource.GetAsset<GameObject>("Arrow");

        ArrowBlock newArrow = Object.Instantiate(prefab).GetComponent<ArrowBlock>();

        pool.Add(newArrow);

        return newArrow;
    }


    public T GetSceneUI<T>() where T : SceneUI
    {
        string key = typeof(T).Name;

        if (scenePool.TryGetValue(key, out SceneUI sceneUI))
        {
            return sceneUI as T;
        }

        return null;
    }

    public void PushSceneUI<T>(SceneUI sceneUI)
    {
        string key = typeof(T).Name;

        scenePool[key] = sceneUI;
    }


    public T GetPopupUI<T>(string key) where T : PopupUI
    {
        if (popUIPool.TryGetValue(key, out PopupUI popup))
        {
            return popup as T;
        }

        return null;
    }

    public void PushPopupUI<T>(PopupUI popupUI)
    {
        string key = typeof(T).Name;

        popUIPool[key] = popupUI;
    }



}