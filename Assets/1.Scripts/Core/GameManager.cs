using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField]
    private GridManager gridManager;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        gridManager.OnAllBlocksRemoved += ClearStage;
    }

    private void ClearStage()
    {
        Debug.Log("Stage Clear");
    }
}