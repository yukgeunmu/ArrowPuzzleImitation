using Codice.CM.Common;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public class ArrowData
{
    public List<Vector3> Cells = new();

    public Direction HeadDirection;
}


public class StagePathEditorWindow : EditorWindow
{
    private const float MinZoom = 10f;
    private const float MaxZoom = 100f;

    private float cellSize = 50f;

    private int width = 10;
    private int height = 10;

    private Texture2D referenceImage;

    private Vector2 scrollPos;


    private readonly List<Vector3> currentPath = new();
    private readonly List<ArrowData> arrows = new();
    private ArrowData selectedArrow;

    private bool isDragging;

    private StageDataSO currentStage;


    [MenuItem("Tools/Arrow Puzzle/Stage Path Editor")]
    public static void Open()
    {
        GetWindow<StagePathEditorWindow>();
    }

    private void OnGUI()
    {
        DrawToolbar();

        GUILayout.Space(10);

        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

        DrawCanvas();

        HandleZoom(Event.current);

        EditorGUILayout.EndScrollView();

        HandleKeyboardInput();
    }

    private void DrawToolbar()
    {
        width =
            EditorGUILayout.IntField(
                "Width",
                width);

        height =
            EditorGUILayout.IntField(
                "Height",
                height);

        referenceImage =
            (Texture2D)EditorGUILayout.ObjectField(
                "Reference Image",
                referenceImage,
                typeof(Texture2D),
                false);

        currentStage = (StageDataSO)EditorGUILayout.ObjectField( "Stage",currentStage, typeof(StageDataSO), false);

        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("Save"))
        {
            SaveStage();
        }

        if (GUILayout.Button("Load"))
        {
            LoadStage();

            GUI.FocusControl(null);
        }

        if (GUILayout.Button("Create Stage"))
        {
            CreateStageAsset();
        }

        GUI.enabled = selectedArrow != null;

        if (GUILayout.Button("Delete Selected Arrow"))
        {
            arrows.Remove(selectedArrow);
            selectedArrow = null;
        }

        GUI.enabled = true;

        EditorGUILayout.EndHorizontal();


        GUILayout.Space(5);

        cellSize =
            EditorGUILayout.Slider(
                "Zoom",
                cellSize,
                10f,
                100f);
    }

    private void DrawCanvas()
    {
        Rect gridRect = GUILayoutUtility.GetRect( width * cellSize, height * cellSize);

        DrawBackground(gridRect);

        DrawGrid(gridRect);

        DrawSavedArrows(gridRect);

        DrawCurrentPath(gridRect);

        HandleGridInput(gridRect, Event.current);
    }

    private void DrawCurrentPath(Rect gridRect)
    {
        if (currentPath.Count < 2)
            return;

        Handles.color = Color.green;

        for (int i = 0; i < currentPath.Count - 1; i++)
        {
            Vector2 p1 = GetCellCenter( gridRect,
                    currentPath[i]);

            Vector2 p2 =
                GetCellCenter(
                    gridRect,
                    currentPath[i + 1]);

            Handles.DrawLine(
                p1,
                p2);
        }
    }

    private Vector2 GetCellCenter( Rect gridRect, Vector3 cell)
    {
        return new Vector2(
            gridRect.x +
            cell.x * cellSize +
            cellSize * 0.5f,

            gridRect.y +
            (height - 1 - cell.y) * cellSize +
            cellSize * 0.5f);
    }

    private void DrawBackground(Rect rect)
    {
        if (referenceImage == null)
            return;

        Color old = GUI.color;

        GUI.color =
            new Color(1, 1, 1, 0.3f);

        GUI.DrawTexture(
            rect,
            referenceImage,
            ScaleMode.StretchToFill);

        GUI.color = old;
    }

    private void DrawGrid(Rect gridRect)
    {
        Handles.color = new Color(1, 1, 1, 0.3f);

        for (int x = 0; x <= width; x++)
        {
            float posX =
                gridRect.x +
                x * cellSize;

            Handles.DrawLine(
                new Vector2(posX, gridRect.y),
                new Vector2(posX, gridRect.yMax));
        }

        for (int y = 0; y <= height; y++)
        {
            float posY =
                gridRect.y +
                y * cellSize;

            Handles.DrawLine(
                new Vector2(gridRect.x, posY),
                new Vector2(gridRect.xMax, posY));
        }
    }

    private void HandleZoom(Event e)
    {
        if (!e.control)
            return;

        if (e.type != EventType.ScrollWheel)
            return;

        float zoomDelta = -e.delta.y * 2f;

        cellSize = Mathf.Clamp(
            cellSize + zoomDelta,
            MinZoom,
            MaxZoom);

        Repaint();

        e.Use();
    }

    private void HandleGridInput( Rect gridRect, Event e)
    {
        Vector2 mousePos = e.mousePosition;

        if (!gridRect.Contains(mousePos))
            return;

        if (e.type == EventType.MouseDown && e.button == 0)
        {
            EditorGUI.FocusTextInControl(null);

            currentPath.Clear();

            currentPath.Add(GetCellPosition(gridRect,mousePos));

            isDragging = true;

            e.Use();
        }

        if(isDragging && e.type == EventType.MouseDrag)
        {
            Vector3  cell = GetCellPosition(gridRect, mousePos);

            if (!currentPath.Contains(cell))
            {
                currentPath.Add(cell);
                Repaint();
            }

            e.Use();
        }

        if(isDragging && e.type == EventType.MouseUp)
        {
            isDragging = false;

            CreateArrowFromPath();

            e.Use();
        }

        if (e.type == EventType.MouseDown && e.button == 1)
        {
            selectedArrow =  FindArrowAtPosition(
                    gridRect,
                    mousePos);

            Focus();

            Repaint();

            e.Use();

            return;
        }

        if (Event.current.type == EventType.MouseDown)
        {
            Focus();
        }
    }


    private Vector3 GetCellPosition( Rect gridRect, Vector2 mousePos)
    {
        int x = Mathf.FloorToInt((mousePos.x - gridRect.x) / cellSize);

        int y = height - 1 - Mathf.FloorToInt((mousePos.y - gridRect.y) / cellSize);

        return new Vector3(x, y);
    }


    private Direction GetHeadDirection(List<Vector3> path)
    {
        if (path.Count < 2)
            return Direction.Up;

        Vector3 head = path[^1];

        Vector3 prev = path[^2];

        Vector3 dir = head - prev;

        if (dir == Vector3.up)
            return Direction.Up;

        if (dir == Vector3.down)
            return Direction.Down;

        if (dir == Vector3.left)
            return Direction.Left;

        return Direction.Right;
    }

    private void CreateArrowFromPath()
    {
        if (currentPath.Count < 2)
            return;

        ArrowData arrow = new();

        arrow.Cells = new List<Vector3>(currentPath);

        arrow.HeadDirection = GetHeadDirection(currentPath);

        arrows.Add(arrow);

        currentPath.Clear();

        Repaint();
    }

    private void DrawSavedArrows(Rect gridRect)
    {
        foreach (var arrow in arrows)
        {
            Handles.color = arrow == selectedArrow ? Color.red : Color.yellow;

            for (int i = 0; i < arrow.Cells.Count - 1; i++)
            {
                Vector2 p1 = GetCellCenter( gridRect, arrow.Cells[i]);

                Vector2 p2 = GetCellCenter( gridRect,arrow.Cells[i + 1]);

                Handles.DrawLine( p1, p2);
            }

            Vector2 headPos = GetCellCenter(gridRect, arrow.Cells[^1]);

            DrawArrowHead(headPos, arrow.HeadDirection);
        }
    }

    private ArrowData FindArrowAtPosition( Rect gridRect, Vector2 mousePos)
    {
        foreach (var arrow in arrows)
        {
            for (int i = 0;i < arrow.Cells.Count - 1; i++)
            {
                Vector2 p1 =  GetCellCenter( gridRect, arrow.Cells[i]);

                Vector2 p2 = GetCellCenter( gridRect,  arrow.Cells[i + 1]);

                float distance = DistanceToSegment( mousePos, p1, p2);

                if (distance < 10f)
                    return arrow;
            }
        }

        return null;
    }

    private float DistanceToSegment( Vector2 p, Vector2 a,Vector2 b)
    {
        Vector2 ab = b - a;

        float t = Vector2.Dot( p - a, ab) / ab.sqrMagnitude;

        t = Mathf.Clamp01(t);

        Vector2 closest = a + ab * t;

        return Vector2.Distance(
            p,
            closest);
    }



    private void HandleKeyboardInput()
    {
        Event e = Event.current;

        if (selectedArrow == null)
            return;


        if (e.type == EventType.KeyDown && e.keyCode == KeyCode.Delete)
        {
            arrows.Remove(selectedArrow);

            selectedArrow = null;

            Repaint();

            e.Use();
        }
    }

    private void SaveStage()
    {
        if (currentStage == null)
            return;

        currentStage.Width = width;
        currentStage.Height = height;

        currentStage.Blocks = new();

        foreach (var arrow in arrows)
        {
            BlockInfo data = new();

            data.Type = BlockType.Arrow;

            data.HeadDirection =
                arrow.HeadDirection;

            data.Cells = new();

            data.Position = Vector3.zero;

            foreach (var cell in arrow.Cells)
            {
                data.Cells.Add(
                    new Vector3(
                        cell.x,
                        cell.y));
            }

            currentStage.Blocks.Add(data);
        }

        EditorUtility.SetDirty(currentStage);

        AssetDatabase.SaveAssets();

        Debug.Log("Stage Saved");
    }
    

    //저장된 스테이지 불러오기
    private void LoadStage()
    {
        if (currentStage == null)
            return;

        width = currentStage.Width;
        height = currentStage.Height;

        arrows.Clear();

        foreach (var data in currentStage.Blocks)
        {
            ArrowData arrow = new();

            arrow.HeadDirection = data.HeadDirection;

            arrow.Cells = new();

            foreach (var cell in data.Cells)
            {
                arrow.Cells.Add( new Vector3( (int)cell.x,(int)cell.y));
            }

            arrows.Add(arrow);
        }

        Focus();

        Selection.activeObject = null;

        Repaint();

        Debug.Log("Stage Loaded");
    }


    //스테이지 에셋 파일 생성
    private void CreateStageAsset()
    {
        StageDataSO stage = ScriptableObject.CreateInstance<StageDataSO>();

        string path = EditorUtility.SaveFilePanelInProject(
                "Create Stage",
                "Stage",
                "asset",
                "");

        if (string.IsNullOrEmpty(path))
            return;

        AssetDatabase.CreateAsset( stage, path);

        AssetDatabase.SaveAssets();

        currentStage = stage;
    }


    //화살 머리 그리기 함수
    private void DrawArrowHead( Vector2 position, Direction direction)
    {
        const float size = 10f;

        int reversNum = 1;

        Vector2 forward = Vector2.up;

        switch (direction)
        {
            case Direction.Up:
                forward = Vector2.up;
                reversNum = -1;
                break;

            case Direction.Down:
                forward = Vector2.down;
                reversNum = -1;
                break;

            case Direction.Left:
                forward = Vector2.left;
                break;

            case Direction.Right:
                forward = Vector2.right;
                break;
        }



        Vector2 left =
            reversNum * (Quaternion.Euler(0, 0, 150) *
            forward *
            size);

        Vector2 right =
            reversNum * (Quaternion.Euler(0, 0, -150) *
            forward *
            size);

        Handles.DrawLine(
            position,
            position + left);

        Handles.DrawLine(
            position,
            position + right);
    }
}