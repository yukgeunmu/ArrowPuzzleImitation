using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class StageEditorWindow : EditorWindow
{
    private int width = 8;
    private int height = 8;

    private EditorToolType currentTool;

    private readonly List<ArrowEditorData> arrows = new();
    private readonly List<Vector3> obstacles = new();

    private ArrowEditorData selectedArrow;

    private bool isDragging;
    private readonly List<Vector3> dragCells = new();

    [MenuItem("Tools/Arrow Puzzle/Stage Editor")]
    public static void Open()
    {
        GetWindow<StageEditorWindow>();
    }

    private void OnGUI()
    {
        DrawHeader();

        GUILayout.Space(10);

        DrawGrid();

        GUILayout.Space(20);

        DrawSelectedArrow();

        GUILayout.Space(20);

        if (GUILayout.Button("Save Stage"))
        {
            SaveStage();
        }


    }

    private void DrawHeader()
    {
        width = EditorGUILayout.IntField("Width", width);
        height = EditorGUILayout.IntField("Height", height);

        currentTool =
            (EditorToolType)GUILayout.Toolbar(
                (int)currentTool,
                new[]
                {
                    "Arrow",
                    "Obstacle",
                    "Erase"
                });
    }

    private void DrawGrid()
    {
        Event e = Event.current;

        for (int y = height - 1; y >= 0; y--)
        {
            EditorGUILayout.BeginHorizontal();

            for (int x = 0; x < width; x++)
            {
                Vector3 pos = new(x, y);

                Rect rect =
                    GUILayoutUtility.GetRect(
                        50,
                        50);

                GUI.backgroundColor =
                    GetCellColor(pos);

                GUI.Box(
                    rect,
                    GetCellLabel(pos));

                GUI.backgroundColor = Color.white;

                HandleCellInput(
                    rect,
                    pos,
                    e);
            }

            EditorGUILayout.EndHorizontal();
        }

        if (e.type == EventType.MouseUp)
        {
            FinishDrag();
        }
    }

    private void HandleCellInput(
        Rect rect,
        Vector3 pos,
        Event e)
    {
        if (!rect.Contains(e.mousePosition))
            return;

        switch (currentTool)
        {
            case EditorToolType.Arrow:

                if (e.type == EventType.MouseDown)
                {
                    isDragging = true;
                    dragCells.Clear();

                    AddDragCell(pos);

                    e.Use();
                }

                if (e.type == EventType.MouseDrag && isDragging)
                {
                    AddDragCell(pos);

                    Repaint();

                    e.Use();
                }

                break;

            case EditorToolType.Obstacle:

                if (e.type == EventType.MouseDown)
                {
                    if (!obstacles.Contains(pos))
                    {
                        obstacles.Add(pos);
                    }

                    Repaint();

                    e.Use();
                }

                break;

            case EditorToolType.Erase:

                if (e.type == EventType.MouseDown)
                {
                    RemoveAt(pos);

                    Repaint();

                    e.Use();
                }

                break;
        }
    }

    private void AddDragCell(Vector3 pos)
    {
        if (!dragCells.Contains(pos))
        {
            dragCells.Add(pos);
        }
    }

    private void FinishDrag()
    {
        if (!isDragging)
            return;

        isDragging = false;

        if (dragCells.Count < 2)
        {
            dragCells.Clear();
            return;
        }

        ArrowEditorData arrow = new();

        arrow.Cells.AddRange(dragCells);

        arrows.Add(arrow);

        selectedArrow = arrow;

        dragCells.Clear();

        Repaint();
    }

    private void DrawSelectedArrow()
    {
        if (selectedArrow == null)
            return;

        EditorGUILayout.LabelField(
            "Selected Arrow",
            EditorStyles.boldLabel);

        selectedArrow.HeadDirection =
            (Direction)EditorGUILayout.EnumPopup(
                "Head Direction",
                selectedArrow.HeadDirection);
    }

    private Color GetCellColor(Vector3 pos)
    {
        if (dragCells.Contains(pos))
            return Color.yellow;

        if (obstacles.Contains(pos))
            return Color.gray;

        foreach (var arrow in arrows)
        {
            if (arrow.Cells.Contains(pos))
                return Color.green;
        }

        return Color.white;
    }

    private string GetCellLabel(Vector3 pos)
    {
        if (obstacles.Contains(pos))
            return "¡á";

        foreach (var arrow in arrows)
        {
            if (arrow.Cells.Count == 0)
                continue;

            if (arrow.Cells[^1] == pos)
            {
                return GetDirectionLabel(
                    arrow.HeadDirection);
            }

            if (arrow.Cells.Contains(pos))
            {
                return "¡Ü";
            }
        }

        return "";
    }

    private string GetDirectionLabel(
        Direction direction)
    {
        return direction switch
        {
            Direction.Up => "¡è",
            Direction.Down => "¡é",
            Direction.Left => "¡ç",
            Direction.Right => "¡æ",
            _ => ""
        };
    }

    private void RemoveAt(Vector3 pos)
    {
        obstacles.Remove(pos);

        for (int i = arrows.Count - 1; i >= 0; i--)
        {
            if (arrows[i].Cells.Contains(pos))
            {
                arrows.RemoveAt(i);
            }
        }
    }

    private void SaveStage()
    {
        StageDataSO stage =
            ScriptableObject.CreateInstance<StageDataSO>();

        stage.Width = width;
        stage.Height = height;

        foreach (var arrow in arrows)
        {
            BlockInfo info = new();

            info.Type = BlockType.Arrow;

            info.Cells =
                new List<Vector3>(
                    arrow.Cells);

            info.HeadDirection =
                arrow.HeadDirection;

            stage.Blocks.Add(info);
        }

        foreach (var obstacle in obstacles)
        {
            BlockInfo info = new();

            info.Type = BlockType.Obstacle;

            info.Position = obstacle;

            stage.Blocks.Add(info);
        }

        string path =
            EditorUtility.SaveFilePanelInProject(
                "Save Stage",
                "Stage",
                "asset",
                "");

        if (string.IsNullOrEmpty(path))
            return;

        AssetDatabase.CreateAsset(
            stage,
            path);

        AssetDatabase.SaveAssets();

        AssetDatabase.Refresh();

        Debug.Log("Stage Saved");
    }
}

public enum EditorToolType
{
    Arrow,
    Obstacle,
    Erase
}