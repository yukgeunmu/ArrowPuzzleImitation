using NUnit.Framework;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Audio;

public class StagePathEditorWindow : EditorWindow
{
    private const float MinZoom = 10f;
    private const float MaxZoom = 100f;

    private float cellSize = 50f;

    private int width = 10;
    private int height = 10;
    private int minLen = 2;
    private int maxLen = 5;

    private Texture2D referenceImage;

    private Vector2 scrollPos;


    private readonly List<Vector3> currentPath = new();
    private List<ArrowData> arrows = new();
    private ArrowData selectedArrow;

    private bool isDragging;

    private StageDataSO currentStage;

    private readonly Stack<EditorAction> undoStack = new();

    private readonly Stack<EditorAction> redoStack = new();

    private bool? solveResult;

    private int solveDepth;

    private string solveDifficulty;

    private List<int> solutionPath = new();

    private int targetMoves = 10;
    private int generateTryCount = 100;


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

        minLen =
            EditorGUILayout.IntField(
                "MinLength",
                minLen);

        maxLen =
            EditorGUILayout.IntField(
                "MaxLength",
                maxLen);

        referenceImage =
            (Texture2D)EditorGUILayout.ObjectField(
                "Reference Image",
                referenceImage,
                typeof(Texture2D),
                false);

        currentStage = (StageDataSO)EditorGUILayout.ObjectField("Stage", currentStage, typeof(StageDataSO), false);

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

        EditorGUILayout.Space();

        targetMoves =
            EditorGUILayout.IntField(
                "Target Moves",
                targetMoves);

        generateTryCount =
            EditorGUILayout.IntField(
                "Max Retry",
                generateTryCount);

        if (GUILayout.Button("Generate"))
        {
            GenerateStage();
        }



        if (GUILayout.Button("Solve"))
        {
            RunSolver();
        }


        GUILayout.Space(5);

        cellSize =
            EditorGUILayout.Slider(
                "Zoom",
                cellSize,
                10f,
                100f);


        if (solveResult.HasValue)
        {
            EditorGUILayout.Space();

            GUI.color =
                solveResult.Value
                    ? Color.green
                    : Color.red;

            EditorGUILayout.LabelField(
                solveResult.Value
                    ? "Clearable"
                    : "Impossible");

            GUI.color = Color.white;

            EditorGUILayout.LabelField(solveDifficulty);

            if (solveResult.Value)
            {
                EditorGUILayout.LabelField(
                    $"Depth : {solveDepth}");
            }

            if (solutionPath.Count > 0)
            {
                string text =
                    string.Join(
                        " -> ",
                        solutionPath);

                EditorGUILayout.HelpBox(
                    text,
                    MessageType.Info);
            }
        }
    }

    private void DrawCanvas()
    {
        Rect gridRect = GUILayoutUtility.GetRect(width * cellSize, height * cellSize);

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
            Vector2 p1 = GetCellCenter(gridRect,
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

    private Vector2 GetCellCenter(Rect gridRect, Vector3 cell)
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

    private void HandleGridInput(Rect gridRect, Event e)
    {
        Vector2 mousePos = e.mousePosition;

        if (!gridRect.Contains(mousePos))
            return;

        if (e.type == EventType.MouseDown && e.button == 0)
        {
            EditorGUI.FocusTextInControl(null);

            currentPath.Clear();

            currentPath.Add(GetCellPosition(gridRect, mousePos));

            isDragging = true;

            e.Use();
        }

        if (isDragging && e.type == EventType.MouseDrag)
        {
            Vector3 cell = GetCellPosition(gridRect, mousePos);

            if (!currentPath.Contains(cell))
            {
                currentPath.Add(cell);
                Repaint();
            }

            e.Use();
        }

        if (isDragging && e.type == EventType.MouseUp)
        {
            isDragging = false;

            CreateArrowFromPath();

            e.Use();
        }

        if (e.type == EventType.MouseDown && e.button == 1)
        {
            selectedArrow = FindArrowAtPosition(
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


    // 마우스 드래그 시 셀 위치 반환
    private Vector3 GetCellPosition(Rect gridRect, Vector2 mousePos)
    {
        int x = Mathf.FloorToInt((mousePos.x - gridRect.x) / cellSize);

        int y = height - 1 - Mathf.FloorToInt((mousePos.y - gridRect.y) / cellSize);

        return new Vector3(x, y);
    }

    // 머리 방향 설정
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


    // 생성한 화살 리스트에 추가
    private void CreateArrowFromPath()
    {
        if (currentPath.Count < 2)
            return;

        ArrowData arrow = new();

        arrow.Cells = new List<Vector3>(currentPath);

        arrow.HeadDirection = GetHeadDirection(currentPath);

        arrows.Add(arrow);

        undoStack.Push(new EditorAction
        {
            Type = EditorActionType.CreateArrow,
            Arrow = arrow
        });

        redoStack.Clear();

        currentPath.Clear();

        Repaint();
    }

    //선 그리기
    private void DrawSavedArrows(Rect gridRect)
    {

        for (int i = 0; i < arrows.Count; i++)
        {
            ArrowData arrow = arrows[i];

            Handles.color = arrow == selectedArrow ? Color.red : Color.yellow;

            for (int j = 0; j < arrows[i].Cells.Count - 1; j++)
            {
                Vector2 p1 = GetCellCenter(gridRect, arrow.Cells[j]);

                Vector2 p2 = GetCellCenter(gridRect, arrow.Cells[j + 1]);

                Handles.DrawLine(p1, p2);
            }

            Vector2 headPos = GetCellCenter(gridRect, arrow.Cells[^1]);

            GUI.Label(new Rect(headPos.x - 20, headPos.y - 20, 30, 20), i.ToString());

            DrawArrowHead(headPos, arrow.HeadDirection);
        }

    }

    //그리드에 화살 위치 감지
    private ArrowData FindArrowAtPosition(Rect gridRect, Vector2 mousePos)
    {
        foreach (var arrow in arrows)
        {
            for (int i = 0; i < arrow.Cells.Count - 1; i++)
            {
                Vector2 p1 = GetCellCenter(gridRect, arrow.Cells[i]);

                Vector2 p2 = GetCellCenter(gridRect, arrow.Cells[i + 1]);

                float distance = DistanceToSegment(mousePos, p1, p2);

                if (distance < 10f)
                    return arrow;
            }
        }

        return null;
    }

    // 마우스위치와 선 위치 거리 감지
    private float DistanceToSegment(Vector2 p, Vector2 a, Vector2 b)
    {
        Vector2 ab = b - a;

        float t = Vector2.Dot(p - a, ab) / ab.sqrMagnitude;

        t = Mathf.Clamp01(t);

        Vector2 closest = a + ab * t;

        return Vector2.Distance(
            p,
            closest);
    }

    // 키입력
    private void HandleKeyboardInput()
    {
        Event e = Event.current;

        if (e.type != EventType.KeyDown)
            return;

        if (e.control && e.keyCode == KeyCode.Z)
        {
            Undo();

            e.Use();
        }

        if (e.control && e.keyCode == KeyCode.Y)
        {
            Redo();

            e.Use();
        }


        if (selectedArrow == null)
            return;

        // 화살 제거
        if (e.type == EventType.KeyDown && e.keyCode == KeyCode.Delete)
        {
            undoStack.Push(new EditorAction
            {
                Type = EditorActionType.DeleteArrow,
                Arrow = selectedArrow
            });

            redoStack.Clear();

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
                arrow.Cells.Add(new Vector3((int)cell.x, (int)cell.y));
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

        AssetDatabase.CreateAsset(stage, path);

        AssetDatabase.SaveAssets();

        currentStage = stage;
    }


    //화살 머리 그리기 함수
    private void DrawArrowHead(Vector2 position, Direction direction)
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


    // 되돌리기
    private void Undo()
    {
        if (undoStack.Count == 0)
            return;

        EditorAction action = undoStack.Pop();

        switch (action.Type)
        {
            case EditorActionType.CreateArrow:

                arrows.Remove(action.Arrow);

                break;

            case EditorActionType.DeleteArrow:

                arrows.Add(action.Arrow);

                break;
        }

        redoStack.Push(action);

        Repaint();
    }

    private void Redo()
    {
        if (redoStack.Count == 0)
            return;

        EditorAction action = redoStack.Pop();

        switch (action.Type)
        {
            case EditorActionType.CreateArrow:

                arrows.Add(action.Arrow);

                break;

            case EditorActionType.DeleteArrow:

                arrows.Remove(action.Arrow);

                break;
        }

        undoStack.Push(action);

        Repaint();
    }

    private void RunSolver()
    {
        SolverState state = BuildSolverState();

        SolverResult result = PuzzleSolver.ValidateGeneratedPuzzle(state);

        solveResult = result.CanSolve;

        solveDepth = result.MinMoves;

        solveDifficulty = result.Difficulty;

        Repaint();
    }


    //SolverState 변환
    private SolverState BuildSolverState()
    {
        int id = 0;
        SolverState state = new();

        state.Width = width;
        state.Height = height;

        foreach (ArrowData data in arrows)
        {
            SolverArrow arrow = new();

            arrow.Id = id;

            arrow.HeadDirection = data.HeadDirection;

            foreach (var cell in data.Cells)
            {
                arrow.Cells.Add(
                    new Vector3(
                        cell.x,
                        cell.y,
                        0));

            }

            id++;

            state.Arrows.Add(arrow);
        }

        return state;
    }

    private void GenerateStage()
    {
        AutoGeneratorReverse autoGeneratorReverse = new();
        arrows = autoGeneratorReverse.Generate(width, height, minLen, maxLen);

        Repaint();
    }


}

