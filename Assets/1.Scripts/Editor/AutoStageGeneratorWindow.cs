using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class AutoStageGeneratorWindow : EditorWindow
{
    private int width = 10;
    private int height = 10;

    private int minLength = 2;
    private int maxLength = 6;

    private List<ArrowData> arrows = new();

    private StageDataSO currentStage;

    [MenuItem("Tools/Arrow Puzzle/Auto Generator")]
    public static void Open()
    {
        GetWindow<AutoStageGeneratorWindow>();
    }

    private void OnGUI()
    {
        currentStage = (StageDataSO)EditorGUILayout.ObjectField("Stage", currentStage, typeof(StageDataSO), false);

        width =
            EditorGUILayout.IntField(
                "Width",
                width);

        height =
            EditorGUILayout.IntField(
                "Height",
                height);

        minLength =
            EditorGUILayout.IntField(
                "Min Length",
                minLength);

        maxLength =
            EditorGUILayout.IntField(
                "Max Length",
                maxLength);

        if (GUILayout.Button("Generate"))
        {
            Generate();
        }

        if (GUILayout.Button("ReverseGenerate"))
        {
            ReverseGenerate();
        }

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

        GUI.enabled = true;

        EditorGUILayout.EndHorizontal();
    }


    private void ReverseGenerate()
    {
        PairGenerator generator = new PairGenerator();

        arrows = generator.Generate(width, height);

        Debug.Log("Complete");
    }



    private void Generate()
    {
        List<Region> regions = GeneratePartition();

        arrows = ConvertRegionsToArrows(regions);

        Debug.Log($"Arrow Count : {arrows.Count}");
    }

    private List<Region> GeneratePartition()
    {
        List<Region> regions = new();

        bool[,] visited = new bool[width, height];

        while (true)
        {
            Vector3 start = FindUnvisitedCell(visited);

            if (start.x == -1)
                break;

            Region region = CreateRegion(start, visited);

            regions.Add(region);
        }

        FixSmallRegions(regions);

        return regions;
    }

    private Vector3 FindUnvisitedCell(bool[,] visited)
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (!visited[x, y])
                    return new Vector3(x, y);
            }
        }

        return new Vector3(-1, -1);
    }

    private Region CreateRegion(Vector3 start, bool[,] visited)
    {
        Region region = new();

        int targetSize = UnityEngine.Random.Range(minLength, maxLength + 1);

        Queue<Vector3> queue = new();

        queue.Enqueue(start);

        visited[(int)start.x, (int)start.y] = true;

        while (queue.Count > 0 && region.Cells.Count < targetSize)
        {
            Vector3 current = queue.Dequeue();

            region.Cells.Add(current);

            List<Vector3> neighbours = GetFreeNeighbours(current, visited);

            Shuffle(neighbours);

            foreach (var next in neighbours)
            {
                if (region.Cells.Count >= targetSize)
                    break;

                visited[(int)next.x, (int)next.y] = true;

                queue.Enqueue(next);
            }
        }

        return region;
    }

    private List<Vector3> GetFreeNeighbours(Vector3 current, bool[,] visited)
    {
        List<Vector3> result = new();

        Vector3[] dirs =
        {
            Vector3.up,
            Vector3.down,
            Vector3.left,
            Vector3.right
        };

        foreach (var dir in dirs)
        {
            Vector3 next = current + dir;

            if (next.x < 0 || next.x >= width)
                continue;

            if (next.y < 0 || next.y >= height)
                continue;

            if (visited[(int)next.x, (int)next.y])
                continue;

            result.Add(next);
        }

        return result;
    }

    private void Shuffle(List<Vector3> neighbours)
    {
        // 리스트가 비어있거나 원소가 1개뿐이면 섞을 필요가 없음
        if (neighbours == null || neighbours.Count <= 1) return;

        // 뒤에서부터 앞으로 가면서 요소를 무작위로 바꿉니다.
        for (int i = neighbours.Count - 1; i > 0; i--)
        {
            // 0부터 i 사이의 랜덤한 인덱스를 선택
            int randomIndex = UnityEngine.Random.Range(0, i + 1);

            // 현재 위치(i)의 값과 랜덤 위치(randomIndex)의 값을 스왑(Swap)
            Vector3 temp = neighbours[i];
            neighbours[i] = neighbours[randomIndex];
            neighbours[randomIndex] = temp;
        }
    }

    private void FixSmallRegions(List<Region> regions)
    {
        for (int i = regions.Count - 1; i >= 0; i--)
        {
            if (regions[i].Cells.Count >= 2)
                continue;

            Region small = regions[i];

            Region nearest = FindNearestRegion(small.Cells[0], regions);

            nearest.Cells.AddRange(small.Cells);

            regions.RemoveAt(i);
        }
    }

    private Region FindNearestRegion(Vector3 sourceCell, List<Region> allRegions)
    {
        Region nearestRegion = null;
        float minDistance = float.MaxValue;

        for (int i = 0; i < allRegions.Count; i++)
        {
            Region targetRegion = allRegions[i];

            // 1. 자기 자신 구역이거나, 이미 지워질 예정인 작은 구역(칸이 1개 이하)은 후보에서 제외
            if (targetRegion.Cells.Count <= 1 || targetRegion.Cells.Contains(sourceCell))
                continue;

            // 2. 대상 구역의 칸들을 돌며 가장 가까운 거리를 계산
            for (int j = 0; j < targetRegion.Cells.Count; j++)
            {
                float distance = Vector3.Distance(sourceCell, targetRegion.Cells[j]);

                // 3. 기존에 찾은 최소 거리보다 더 가깝다면 갱신
                if (distance < minDistance)
                {
                    minDistance = distance;
                    nearestRegion = targetRegion;
                }
            }
        }

        // 만약 마땅한 이웃을 못 찾았다면(모든 구역이 다 작거나 없을 때) null 반환 예외 처리
        return nearestRegion;
    }

    private List<ArrowData> ConvertRegionsToArrows(List<Region> regions)
    {
        List<ArrowData> arrows = new();

        foreach (Region region in regions)
        {
            ArrowData arrow = new();

            List<Vector3> ordered = BuildSnakePath(region.Cells);

            foreach (var cell in ordered)
            {
                arrow.Cells.Add(
                    new Vector3(
                        cell.x,
                        cell.y,
                        0));
            }

            arrow.HeadDirection = GetHeadDirection(arrow.Cells);

            arrows.Add(arrow);
        }

        return arrows;
    }

    private List<Vector3> BuildSnakePath(List<Vector3> cells)
    {
        List<Vector3> path = new();

        HashSet<Vector3> remain = new(cells);

        Vector3 current = cells[0];

        path.Add(current);

        remain.Remove(current);

        while (remain.Count > 0)
        {
            Vector3 next = remain.FirstOrDefault(x => Mathf.Abs(x.x - current.x) + Mathf.Abs(x.y - current.y) == 1);

            if (!remain.Contains(next))
                break;

            path.Add(next);

            remain.Remove(next);

            current = next;
        }

        return path;
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


}


