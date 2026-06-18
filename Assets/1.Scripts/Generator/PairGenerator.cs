using System.Collections.Generic;
using UnityEngine;

public class PairGenerator
{
    private int width;
    private int height;

    public List<ArrowData> Generate(int width, int height)
    {
        this.width = width;
        this.height = height;

        List<Region> regions = CreatePairs();

        MergeRegions(regions);

        return ConvertToArrows(regions);

    }

    private List<Region> CreatePairs()
    {
        List<Vector3> path = BuildFullPath();

        List<Region> regions = new();

        for (int i = 0; i < path.Count; i += 2)
        {
            Region region = new();

            region.Cells.Add(path[i]);

            if (i + 1 < path.Count)
            {
                region.Cells.Add(path[i + 1]);
            }

            regions.Add(region);
        }

        return regions;

    }

    private void MergeRegions(List<Region> regions)
    {
        int i = 0;

        while (i < regions.Count - 1)
        {
            int total = regions[i].Count + regions[i + 1].Count;

            if (total <= 6 && Random.value > 0.2f)
            {
                if (total <= 6 && Random.value > 0.2f)
                {
                    regions[i].Cells.AddRange(regions[i + 1].Cells);

                    regions.RemoveAt(i + 1);
                }
            }
            else
            {
                i++;
            }
        }


    }


    private List<ArrowData> ConvertToArrows(List<Region> regions)
    {
        List<ArrowData> result = new();

        foreach (var region in regions)
        {
            List<Vector3> path = BuildPath(region);

            if (path.Count != region.Cells.Count)
            {
                Debug.LogWarning(
                    $"Path 생성 실패 {path.Count}/{region.Cells.Count}");

                path = new List<Vector3>(region.Cells);
            }

            ArrowData arrow = BuildArrow(path);

            result.Add(arrow);
        }

        return result;
    }

    private List<Vector3> BuildPath(Region region)
    {
        List<Vector3> bestPath = null;

        for (int attempt = 0; attempt < 1000; attempt++)
        {
            List<Vector3> path = new();

            HashSet<Vector3> remain = new(region.Cells);

            Vector3 start = region.Cells[ Random.Range(0, region.Cells.Count)];

            path.Add(start);

            remain.Remove(start);

            if (BuildPathDFS(start, remain, path))
            {
                return path;
            }

            if (bestPath == null || path.Count > bestPath.Count)
            {
                bestPath = new(path);
            }
        }

        return bestPath;
    }

    private List<Vector3> BuildFullPath()
    {
        List<Vector3> result = new();

        for (int y = 0; y < height; y++)
        {
            if (y % 2 == 0)
            {
                for (int x = 0; x < width; x++)
                {
                    result.Add(new Vector3(x, y));
                }
            }
            else
            {
                for (int x = width - 1; x >= 0; x--)
                {
                    result.Add(new Vector3(x, y));
                }
            }
        }

        return result;
    }

    private bool BuildPathDFS(Vector3 current, HashSet<Vector3> remain, List<Vector3> path)
    {
        if (remain.Count == 0)
            return true;

        List<Vector3> neighbours = new();

        foreach (var next in remain)
        {
            float dist =
                Mathf.Abs(current.x - next.x) +
                Mathf.Abs(current.y - next.y);

            if (dist == 1)
                neighbours.Add(next);
        }

        Shuffle(neighbours);

        foreach (var next in neighbours)
        {
            path.Add(next);

            remain.Remove(next);

            if (BuildPathDFS(next, remain, path))
                return true;

            path.RemoveAt(path.Count - 1);

            remain.Add(next);
        }

        return false;
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

    private ArrowData BuildArrow(List<Vector3> path)
    {
        ArrowData arrow = new();

        arrow.Cells.AddRange(path);

        arrow.HeadDirection = GetHeadDirection(path);

        return arrow;
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
}
