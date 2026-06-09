using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class PoolManager
{
    private readonly List<ArrowBlock> pool = new();

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

        ArrowBlock newArrow =  Object.Instantiate(prefab).GetComponent<ArrowBlock>();

        pool.Add(newArrow);

        return newArrow;
    }

}