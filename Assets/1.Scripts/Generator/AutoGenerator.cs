using System.Collections.Generic;
using UnityEngine;

public class AutoGenerator
{
    private int width;
    private int height;
    private int minLen = 2;
    private int maxLen = 5;

    private bool[,] occupied;

    List<ArrowData> arrows = new List<ArrowData>();


    private List<Vector2Int> freeCellsCache = new List<Vector2Int>();
    private List<Vector2Int> pathCache = new List<Vector2Int>();

    private static readonly Vector2Int[] Dirs = {
        Vector2Int.up,
        Vector2Int.down,
        Vector2Int.left,
        Vector2Int.right
    };

    private const int BatchCheckSize = 8;

    public List<ArrowData> Generate(int width, int height, int minLen, int maxLen)
    {
        this.width = width;
        this.height = height;
        this.minLen = minLen;
        this.maxLen = maxLen;

        // 성공할 때까지 안전하게 반복하는 루프 (최대 10회 시도)
        int maxRetries = 15;
        for (int retry = 0; retry < maxRetries; retry++)
        {
            occupied = new bool[width, height];
            arrows.Clear();

            bool success = FillMapWithArrowsAndValidate();

            if (!success || arrows.Count == 0) continue;

            // 최종 배치된 맵 전체 검증
            SolverState state = BuildSolverState();
            SolverResult result = PuzzleSolver.SolveExitOnly(state);

            if (result.CanSolve)
            {
                return arrows;
            }
        }

        Debug.LogWarning("지정한 시도 횟수 내에 완벽하게 풀 수 있는 퍼즐을 생성하지 못했습니다.");
        return arrows;

    }

    private bool FillMapWithArrowsAndValidate()
    {
        bool addedInPass = true;

        List<ArrowData> currentBatchArrows = new List<ArrowData>();

        int totalLoopCount = 0;
        int maxAllowedLoops = 300;

        while (addedInPass)
        {
            totalLoopCount++;
            if (totalLoopCount > maxAllowedLoops) return false; // 너무 오래 걸리면 이 판은 버리고 재생성 유도

            addedInPass = false;
            UpdateFreeCells();

            if (freeCellsCache.Count < minLen) break;

            Shuffle(freeCellsCache);

            foreach (var startCell in freeCellsCache)
            {
                // 긴 화살표부터 시도
                for (int targetLen = maxLen; targetLen >= minLen; targetLen--)
                {
                    pathCache.Clear();
                    pathCache.Add(startCell);

                    if (DFS(startCell, targetLen, pathCache))
                    {
                        // 유효한 경로를 찾았으므로 즉시 화살표로 확정
                        ArrowData newArrow = BuildArrow(pathCache);

                        if (IsExitBlocked(newArrow))
                        {
                            continue; // 막혀있다면 솔버를 돌리지도 않고 탈출 (연산 대폭 절감)
                        }

                        arrows.Add(newArrow);
                        currentBatchArrows.Add(newArrow);

                        // 맵에 점유 표시
                        foreach (var cell in pathCache)
                        {
                            occupied[cell.x, cell.y] = true;
                        }

                        if (currentBatchArrows.Count > BatchCheckSize)
                        {
                            SolverState tempState = BuildSolverState();
                            SolverResult tempResult = PuzzleSolver.SolveExitOnly(tempState);

                            if (tempResult.CanSolve)
                            {
                                // 중간 검증 성공! 이번 묶음은 안전하므로 확정 비우기
                                currentBatchArrows.Clear();
                            }
                            else
                            {
                                // 중간 검증 실패! 이번 묶음에서 추가했던 화살표들을 전부 롤백(제거)
                                foreach (var rollbackArrow in currentBatchArrows)
                                {
                                    arrows.Remove(rollbackArrow);
                                    // 점유했던 타일들도 다시 원상 복구
                                    foreach (var cell in rollbackArrow.Cells)
                                    {
                                        occupied[(int)cell.x, (int)cell.y] = false;
                                    }
                                }
                                currentBatchArrows.Clear();

                                // 실패했으므로 다른 시작점 타일부터 다시 시도하도록 패스 탈출
                                addedInPass = true;
                                break;
                            }

                        }

                        addedInPass = true;
                        break;
                    }
                }
                if (addedInPass) break; // 다시 빈 셀 목록을 갱신하기 위해 루프 탈출
            }
        }

        if (currentBatchArrows.Count > 0)
        {
            SolverState finalState = BuildSolverState();
            SolverResult finalResult = PuzzleSolver.SolveExitOnly(finalState);

            if (!finalResult.CanSolve)
            {
                // 실패 시 찌꺼기 화살표들만 깔끔하게 제거
                foreach (var rollbackArrow in currentBatchArrows)
                {
                    arrows.Remove(rollbackArrow);
                    foreach (var cell in rollbackArrow.Cells)
                    {
                        occupied[(int)cell.x, (int)cell.y] = false;
                    }
                }
            }
        }

        return true;
    }

    private bool IsExitBlocked(ArrowData arrow)
    {
        if (arrow.Cells.Count < 2) return false;

        // 머리 타일과 목 타일의 좌표 추출
        Vector3 head = arrow.Cells[^1];  // BuildArrow에서 path[^1]이 머리 방향이 됨
        Vector3 prev = arrow.Cells[^2];

        int hx = (int)head.x;
        int hy = (int)head.y;
        int nx = (int)prev.x;
        int ny = (int)prev.y;

        // 전진 방향 벡터 구하기
        int dx = hx - nx;
        int dy = hy - ny;

        int cx = hx + dx;
        int cy = hy + dy;

        // 머리가 가리키는 방향으로 레이캐스트(직선 검사)를 날려 맵 끝까지 가봄
        while (cx >= 0 && cx < width && cy >= 0 && cy < height)
        {
            // 나아가는 길목이 이미 다른 화살표로 꽉 막혀있다면 이 화살표는 솔버도 통과 못함
            if (occupied[cx, cy])
                return true;

            cx += dx;
            cy += dy;
        }

        return false; // 출구 방향이 일단 뚫려있음
    }



    private bool DFS(Vector2Int current, int targetLength, List<Vector2Int> path)
    {
        if (path.Count >= targetLength)
            return true;

        List<Vector2Int> neighbours = new List<Vector2Int>(4);
        GetAvailableNeighbours(current, path, neighbours);

        // 이웃 찾기 (인스턴스 생성을 피하기 위해 내부에서 처리하거나 전용 리스트 활용)

        Shuffle(neighbours);

        foreach (var next in neighbours)
        {
            path.Add(next);

            if (DFS(next, targetLength, path))
                return true;

            path.RemoveAt(path.Count - 1);
        }

        return false;
    }

    private void GetAvailableNeighbours(Vector2Int current, List<Vector2Int> currentPath, List<Vector2Int> results)
    {
        results.Clear();

        foreach (var dir in Dirs)
        {
            Vector2Int next = current + dir;

            if (next.x < 0 || next.x >= width || next.y < 0 || next.y >= height)
                continue;

            if (occupied[next.x, next.y])
                continue;

            if (currentPath.Contains(next))
                continue;

            results.Add(next);
        }
    }

    private void UpdateFreeCells()
    {
        freeCellsCache.Clear();
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (!occupied[x, y])
                    freeCellsCache.Add(new Vector2Int(x, y));
            }
        }
    }


    private void Shuffle(List<Vector2Int> list)
    {
        if (list == null || list.Count <= 1) return;

        for (int i = list.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            Vector2Int temp = list[i];
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


    private SolverState BuildSolverState()
    {
        int id = 0;
        SolverState state = new SolverState { Width = width, Height = height };

        foreach (ArrowData data in arrows)
        {
            SolverArrow arrow = new SolverArrow
            {
                Id = id++,
                HeadDirection = data.HeadDirection
            };

            foreach (var cell in data.Cells)
            {
                arrow.Cells.Add(new Vector3(cell.x, cell.y, 0));
            }
            state.Arrows.Add(arrow);
        }

        return state;
    }

}
