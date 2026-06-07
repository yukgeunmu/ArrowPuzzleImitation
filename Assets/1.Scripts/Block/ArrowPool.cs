using System.Collections.Generic;
using UnityEngine;

public class ArrowPool : MonoBehaviour
{
    public static ArrowPool Instance;

    [SerializeField]
    private ArrowBlock prefab;

    private readonly List<ArrowBlock> pool = new();
    private void Awake()
    {
        Instance = this;
    }

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

        ArrowBlock newArrow = Instantiate(prefab);

        pool.Add(newArrow);

        return newArrow;
    }

}