using System.Collections.Generic;
using UnityEngine;

public class ReverseGenerator
{
    private int width;
    private int height;

    private bool[,] occupied;

    private List<ArrowData> arrows;

    public List<ArrowData> Generate(int width, int height)
    {
        this.width = width;
        this.height = height;

        occupied = new bool[width, height];

        arrows = new List<ArrowData>();

        while (HasEmptyCell())
        {
            CreateArrow();
        }

        return arrows;
    }

    private void CreateArrow()
    {
        Vector3 start = GetRandomEmptyCell();

        if (IsIsolatedCell(start))
            return;

        int targetLength = Random.Range(2, 7);

        List<Vector3> path = null;

        for (int length = 6; length >= 2; length--)
        {
            path = GeneratePath(start, length);

            if (path.Count >= 2)
                break;
        }

        if (path == null || path.Count < 2)
        {
            return;
        }

        ArrowData arrow = BuildArrow(path);

        arrows.Add(arrow);

        foreach (var cell in path)
        {
            occupied[(int)cell.x, (int)cell.y] = true;
        }
    }

    private List<Vector3> GeneratePath( Vector3 start, int targetLength)
    {
        List<Vector3> path = new();

        path.Add(start);

        DFS( start, targetLength, path);

        return path;
    }

    private bool DFS( Vector3 current, int targetLength, List<Vector3> path)
    {
        if (path.Count >= targetLength)
            return true;

        List<Vector3> neighbours = GetAvailableNeighbours(current, path);

        Shuffle(neighbours);

        foreach (var next in neighbours)
        {
            path.Add(next);

            if (DFS( next, targetLength, path))
                return true;

            path.RemoveAt(
                path.Count - 1);
        }

        return false;
    }

    private ArrowData BuildArrow(List<Vector3> path)
    {
        ArrowData arrow = new();

        foreach (var cell in path)
        {
            arrow.Cells.Add(
                new Vector3(
                    cell.x,
                    cell.y,
                    0));
        }

        arrow.HeadDirection =  GetHeadDirection(arrow.Cells);

        return arrow;
    }

    private bool HasEmptyCell()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y< height; y++)
            {
                if (!occupied[x, y])
                    return true;
            }
        }

        return false;
    }

    private Vector3 GetRandomEmptyCell()
    {
        List<Vector3> emptyCellList = new List<Vector3>();

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (!occupied[x, y])
                    emptyCellList.Add(new Vector3(x, y));
            }
        }

        if (emptyCellList.Count == 0) return Vector3.zero;

        int randomIndex = UnityEngine.Random.Range(0, emptyCellList.Count);

        return emptyCellList[randomIndex];
    }

    private List<Vector3> GetAvailableNeighbours(Vector3 current, List<Vector3> currentPath)
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

            if (occupied[(int)next.x, (int)next.y])
                continue;

            if (currentPath.Contains(next))
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

    private bool IsIsolatedCell(Vector3 pos)
    {
        int count = 0;

        Vector3[] dirs =
        {
        Vector3.up,
        Vector3.down,
        Vector3.left,
        Vector3.right
    };

        foreach (var dir in dirs)
        {
            Vector3 next = pos + dir;

            if (next.x < 0 || next.x >= width)
                continue;

            if (next.y < 0 || next.y >= height)
                continue;

            if (!occupied[(int)next.x, (int)next.y])
                count++;
        }

        return count == 0;
    }


}