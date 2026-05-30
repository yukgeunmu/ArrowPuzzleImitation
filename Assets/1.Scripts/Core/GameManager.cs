using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    public void CheckClear()
    {
        if (FindAnyObjectByType<GridManager>().BlockCount == 0)
        {
            Debug.Log("Stage Clear");
        }
    }
}