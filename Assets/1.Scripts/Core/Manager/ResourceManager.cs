using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class ResourceManager
{
    private readonly Dictionary<string, AsyncOperationHandle> handles = new();

    private readonly Dictionary<string, UnityEngine.Object> assetCache = new();

    private readonly Dictionary<string, object> tableCache = new();

    public async Task<T> LoadAsync<T>(string key) where T : UnityEngine.Object
    {
        if (assetCache.TryGetValue(key, out UnityEngine.Object obj))
        {
            return obj as T;
        }

        AsyncOperationHandle<T> handle = Addressables.LoadAssetAsync<T>(key);

        await handle.Task;

        if (handle.Status != AsyncOperationStatus.Succeeded)
        {
            Debug.LogError($"Load Failed : {key}");
            return null;
        }

        handles[key] = handle;
        assetCache[key] = handle.Result;

        return handle.Result;
    }

    public async Task LoadDataAsync<T>(string label) where T : UnityEngine.Object
    {
        if (tableCache.ContainsKey(label))
            return;

        AsyncOperationHandle<IList<T>> handle = Addressables.LoadAssetsAsync<T>(label, null);

        await handle.Task;

        if (handle.Status != AsyncOperationStatus.Succeeded)
        {
            Debug.LogError($"Load Failed : {label}");

            return;
        }

        Dictionary<string, T> dictionary = new();

        foreach (T asset in handle.Result)
        {
            if (!dictionary.TryAdd(asset.name, asset))
            {
                Debug.LogError($"Duplicate Asset Name : {asset.name}");
            }
        }

        handles[label] = handle;
        tableCache[label] = dictionary;
    }


    public void Release(string key)
    {
        if (!handles.TryGetValue(key, out var handle))
            return;

        Addressables.Release(handle);

        handles.Remove(key);
        assetCache.Remove(key);
        tableCache.Remove(key);
    }

    public void ReleaseAll()
    {
        foreach (var pair in handles)
        {
            Addressables.Release(pair.Value);
        }

        handles.Clear();
        assetCache.Clear();
        tableCache.Clear();
    }


    public async Task<StageDataSO> LoadStageAsync(int stageId)
    {
        string key = $"Stage{stageId}";

        return await LoadAsync<StageDataSO>(key);
    }

    public T GetData<T>(string tableKey, string dataKey) where T : UnityEngine.Object
    {
        if (!tableCache.TryGetValue(tableKey, out object value))
            return null;


        Dictionary<string, T> table = value as Dictionary<string, T>;



        if (table == null)
            return null;

        table.TryGetValue(dataKey, out T data);

        return data;
    }

    public Dictionary<string, T> GetTableData<T>(string key)
    {
        if (tableCache.TryGetValue(key, out object value))
        {
            return value as Dictionary<string, T>;
        }

        Debug.LogError($"Table Cache Not Found : {key}");
        return null;
    }

    public T GetAsset<T>(string key) where T : UnityEngine.Object
    {
        if (assetCache.TryGetValue(key, out Object obj))
        {
            return obj as T;
        }

        Debug.LogError($"Asset Not Found : {key}");
        return null;
    }

}
