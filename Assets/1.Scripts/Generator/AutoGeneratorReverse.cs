using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class AutoGeneratorReverse
{
    private int width;
    private int height;
    private int minLen = 2;
    private int maxLen = 5;

    private bool[,] occupied;
    private List<ArrowData> arrows = new List<ArrowData>();
    private List<Vector2Int> pathCache = new List<Vector2Int>();
    private Color[] arrowColors = {
        Color.darkBlue,
        Color.rebeccaPurple,
        Color.yellowNice,
        Color.violetRed,
        Color.darkOrange,
        Color.darkGreen,
    };


    private Vector2Int[,] flowMap;

    private static readonly Vector2Int[] Dirs = {

        Vector2Int.up,
        Vector2Int.down,
        Vector2Int.left,
        Vector2Int.right

    };

    public List<ArrowData> Generate(int width, int height, int minLen, int maxLen)
    {
        this.width = width;
        this.height = height;
        this.minLen = minLen;
        this.maxLen = maxLen;

        // 제약 조건이 엄격해졌으므로 완벽한 판이 나올 때까지 시도 횟수를 높입니다 (연산은 여전히 매우 빠릅니다)
        for (int retry = 0; retry < 30; retry++)
        {
            occupied = new bool[width, height];
            flowMap = new Vector2Int[width, height];
            arrows.Clear();

            if (BuildPerfectReversePuzzle())
            {
                SolverState state = BuildSolverState();

                if (ValidateGeneratedPuzzle(state))
                {
                    return arrows;
                }
        }

        }

        Debug.LogWarning("퍼즐 생성 실패 (안전한 맵 선별 중). 다시 시도하세요.");

        return arrows;

    }

    private bool BuildPerfectReversePuzzle()
    {

        List<Vector2Int> allCells = new List<Vector2Int>();

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++) allCells.Add(new Vector2Int(x, y));
        }

        int maxGlobalLoops = 1500;
        int loops = 0;

        while (GetFreeCellCount() >= minLen && loops < maxGlobalLoops)
        {

            loops++;
            Shuffle(allCells);
            bool arrowAdded = false;

            foreach (var startCell in allCells)
            {
                if (occupied[startCell.x, startCell.y]) continue;

                // 1. 이 칸에서 뻗어나갈 수 있는 '진짜 검증된 탈출 방향'을 구합니다.
                Vector2Int exitDir = FindGuaranteedExitDirection(startCell);

                if (exitDir == Vector2Int.zero) continue;

                int targetLen = Random.Range(minLen, maxLen + 1);


                pathCache.Clear();
                pathCache.Add(startCell);


                // exitDir의 반대 방향(안쪽 빈 공간)으로 꼬리를 늘립니다.
                if (DFS_Reverse(startCell, -exitDir, targetLen, pathCache))
                {
                    pathCache.Reverse(); // 머리가 끝([^1])으로 가도록 정렬

                    Vector2Int realHeadDir = GetRealHeadDirectionVector(pathCache);

                    for (int i = 0; i < pathCache.Count; i++)
                    {
                        Vector2Int c = pathCache[i];
                        occupied[c.x, c.y] = true;

                        if (i == pathCache.Count - 1)
                            flowMap[c.x, c.y] = realHeadDir;
                        else
                            flowMap[c.x, c.y] = pathCache[i + 1] - c;
                    }

                    // 2.[가장 중요한 필터링] 머리가 향하는 전진 경로에 마주보거나 꼬인 화살표가 없는지 전수조사
                    if (!ValidateFinalArrowExit(pathCache, realHeadDir))
                    {
                        for (int i = 0; i < pathCache.Count; i++)
                        {
                            Vector2Int c = pathCache[i];
                            occupied[c.x, c.y] = false;
                            flowMap[c.x, c.y] = Vector2Int.zero;
                        }
                        continue;
                    }

                    ArrowData arrow = BuildArrow(pathCache);
                    arrows.Add(arrow);
                    arrowAdded = true;
                    break;

                }

            }

            if (!arrowAdded) break;

        }

        float fillRate = (float)(width * height - GetFreeCellCount()) / (width * height);

        return fillRate > 0.55f && arrows.Count > 0;

    }



    private bool DFS_Reverse(Vector2Int current, Vector2Int growthDir, int targetLength, List<Vector2Int> path)
    {
        if (path.Count >= targetLength) return true;

        List<Vector2Int> candidates = new List<Vector2Int>();

        if (path.Count == 1)
        {
            Vector2Int next = current + growthDir;

            if (IsValidFreeCell(next)) candidates.Add(next);
        }


        if (candidates.Count == 0)
        {
            foreach (var dir in Dirs)
            {
                Vector2Int next = current + dir;

                if (IsValidFreeCell(next) && !path.Contains(next)) candidates.Add(next);
            }

            Shuffle(candidates);

        }


        foreach (var next in candidates)
        {
            path.Add(next);

            if (DFS_Reverse(next, growthDir, targetLength, path)) return true;

            path.RemoveAt(path.Count - 1);
        }

        return false;

    }



    private Vector2Int FindGuaranteedExitDirection(Vector2Int cell)
    {
        List<Vector2Int> validDirs = new List<Vector2Int>();

        foreach (var dir in Dirs)
        {
            Vector2Int next = cell + dir;

            // 벽 밖으로 나가는 것은 무조건 안전
            if (next.x < 0 || next.x >= width || next.y < 0 || next.y >= height)
            {
                validDirs.Add(dir);
                continue;
            }

            //[방향 일치 조건 추가]: 이미 배치된 화살표가 존재한다면, 
            // 그 화살표가 탈출하는 방향(flowMap)이 내가 나아가려는 방향(dir)과 '완벽히 일치'할 때만 디딤돌로 인정합니다.
            if (occupied[next.x, next.y] && flowMap[next.x, next.y] == dir)
            {
                validDirs.Add(dir);
            }
        }

        if (validDirs.Count == 0)
        {
            return Vector2Int.zero;
        }


        return validDirs[Random.Range(0, validDirs.Count)];

    }



    /// <summary>
    /// 마주보는 불량 화살표를 완벽히 필터링하는 전진 경로 레이캐스트
    /// </summary>
    private bool ValidateFinalArrowExit(List<Vector2Int> path, Vector2Int exitDir)
    {
        Vector2Int head = path[^1];
        Vector2Int check = head + exitDir;

        // 머리 바로 앞이 내 몸통이면 실패
        if (path.Contains(check)) return false;

        // 내 머리부터 탈출 벽 끝까지 직선 경로 상의 모든 타일을 전수조사
        while (check.x >= 0 && check.x < width && check.y >= 0 && check.y < height)
        {
            if (occupied[check.x, check.y])
            {
                // [결정적 치트키]: 내 앞길을 막고 있는 화살표의 탈출 방향(flowMap)을 조사합니다.
                // 만약 그 녀석의 탈출 방향이 나와 다르거나, 혹은 역방향(`-exitDir`)이라서 내 쪽을 마주보고 있다면 불량 판정!
                if (flowMap[check.x, check.y] == -exitDir)
                {
                    return false;
                }

            }

            check += exitDir; // 벽 끝까지 직진
        }

        return true;

    }

    private bool IsValidFreeCell(Vector2Int cell)
    {
        if (cell.x < 0 || cell.x >= width || cell.y < 0 || cell.y >= height) return false;
        return !occupied[cell.x, cell.y];

    }



    private int GetFreeCellCount()
    {
        int count = 0;

        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                if (!occupied[x, y]) count++;
        return count;
    }



    private void Shuffle<T>(List<T> list)
    {
        if (list == null || list.Count <= 1) return;

        for (int i = list.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);

            T temp = list[i];
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }



    private ArrowData BuildArrow(List<Vector2Int> path)
    {
        ArrowData arrow = new ArrowData();

        foreach (var cell in path)
        {
            arrow.Cells.Add(new Vector3(cell.x, cell.y, 0));
        }

        arrow.HeadDirection = GetHeadDirection(path);

        arrow.Color = arrowColors[UnityEngine.Random.Range(0, arrowColors.Length)];

        return arrow;

    }



    private Direction GetHeadDirection(List<Vector2Int> path)
    {

        if (path.Count < 2) return Direction.Up;

        Vector2Int head = path[^1];

        Vector2Int prev = path[^2];

        Vector2Int dir = head - prev;


        if (dir == Vector2Int.up) return Direction.Up;

        if (dir == Vector2Int.down) return Direction.Down;

        if (dir == Vector2Int.left) return Direction.Left;

        return Direction.Right;

    }

    private Vector2Int GetRealHeadDirectionVector(List<Vector2Int> path)
    {
        Vector2Int head = path[^1];

        Vector2Int prev = path[^2];

        Vector2Int dir = head - prev;

        return dir;
    }

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

    private bool ValidateGeneratedPuzzle(SolverState state)
    {
        int count = state.Arrows.Count;

        while (state.Arrows.Count > 0)
        {
            bool found = false;

            for (int i = 0; i < state.Arrows.Count; i++)
            {
                if (SolverUtility.CanExitArrow(state, i))
                {
                    state = SolverUtility.MoveArrow(state, i);
                    found = true;
                    break;
                }
            }

            if (!found)
                return false;
        }

        return true;
    }


}