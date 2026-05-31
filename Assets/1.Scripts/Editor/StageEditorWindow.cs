using UnityEditor;
using UnityEngine;

public class StageEditorWindow : EditorWindow
{
    private int width = 5;
    private int height = 5;

    private CellType[,] grid;

    [MenuItem("Tools/Arrow Puzzle/Stage Editor")]
    public static void Open()
    {
        GetWindow<StageEditorWindow>();
    }

    private void OnEnable()
    {
        CreateGrid();
    }

    private void CreateGrid()
    {
        grid = new CellType[width, height];
    }

    private void OnGUI()
    {
        width = EditorGUILayout.IntField("Width", width);
        height = EditorGUILayout.IntField("Height", height);

        if (GUILayout.Button("Resize"))
        {
            CreateGrid();
        }

        DrawGrid();

        GUILayout.Space(20);

        if (GUILayout.Button("Save Stage"))
        {
            SaveStage();
        }
    }

    private void DrawGrid()
    {
        for (int y = height - 1; y >= 0; y--)
        {
            EditorGUILayout.BeginHorizontal();

            for (int x = 0; x < width; x++)
            {
                string label = GetCellLabel(grid[x, y]);

                if (GUILayout.Button(label,
                    GUILayout.Width(50),
                    GUILayout.Height(50)))
                {
                    CycleCell(x, y);
                }
            }

            EditorGUILayout.EndHorizontal();
        }
    }

    private void CycleCell(int x, int y)
    {
        int next =
            ((int)grid[x, y] + 1)
            % System.Enum.GetValues(typeof(CellType)).Length;

        grid[x, y] = (CellType)next;
    }

    private string GetCellLabel(CellType type)
    {
        return type switch
        {
            CellType.Empty => "",
            CellType.ArrowUp => "¡è",
            CellType.ArrowDown => "¡é",
            CellType.ArrowLeft => "¡ç",
            CellType.ArrowRight => "¡æ",
            CellType.Obstacle => "¡á",
            _ => ""
        };
    }

    private void SaveStage()
    {
        StageDataSO stage =
            ScriptableObject.CreateInstance<StageDataSO>();

        stage.Width = width;
        stage.Height = height;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                CellType cell = grid[x, y];

                if (cell == CellType.Empty)
                    continue;

                BlockInfo info = new();

                info.Position = new Vector3(x, y);

                switch (cell)
                {
                    case CellType.Obstacle:
                        info.Type = BlockType.Obstacle;
                        break;

                    case CellType.ArrowUp:
                        info.Type = BlockType.Arrow;
                        info.Direction = Direction.Up;
                        break;

                    case CellType.ArrowDown:
                        info.Type = BlockType.Arrow;
                        info.Direction = Direction.Down;
                        break;

                    case CellType.ArrowLeft:
                        info.Type = BlockType.Arrow;
                        info.Direction = Direction.Left;
                        break;

                    case CellType.ArrowRight:
                        info.Type = BlockType.Arrow;
                        info.Direction = Direction.Right;
                        break;
                }

                stage.Blocks.Add(info);
            }
        }

        string path =
            EditorUtility.SaveFilePanelInProject(
                "Save Stage",
                "Stage",
                "asset",
                "");

        AssetDatabase.CreateAsset(stage, path);

        AssetDatabase.SaveAssets();
    }
}