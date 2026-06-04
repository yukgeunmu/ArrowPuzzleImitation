using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowBlock : BlockBase
{
    [SerializeField]
    private LineRenderer lineRenderer;

    [SerializeField]
    private ArrowSegment headVisual;

    public int Id;


    public List<Vector3> Cells { get; private set; }


    public Direction HeadDirection;

    public Vector3 HeadCell => Cells[Cells.Count - 1];

    private Coroutine moveRoutine;

    private bool isMoving;


    public override bool IsMovable =>  true;

    public int LinkGroupId = -1;

    private GridManager gridManager;


    public void Init(List<Vector3> cells,Direction headDirection , GridManager gridManager)
    {
        Cells = new List<Vector3>(cells);

        this.HeadDirection = headDirection;

        this.gridManager = gridManager;

        headVisual.Init(this);

        RefreshVisual();

        //CreateVisual();
    }


    public void TryMove()
    {
        if (isMoving)
            return;

        if (!gridManager.CanMoveShape(this))
        {
            PlayBlockedAnimation();
            return;
        }

        UndoManager.Instance.Execute(new MoveCommand(this));
           
    }

    public void StartMove()
    {
        if (isMoving)
            return;

       moveRoutine = StartCoroutine(MoveRoutine());
    }

    private IEnumerator MoveRoutine()
    {
        isMoving = true;

        while (true)
        {
            Vector3 nextPos =
                HeadCell +
                HeadDirection.ToVector();

            if (gridManager.IsCompletelyOut(this))
            {
                ExitGrid();
                break;
            }

            if (!gridManager.CanMoveShape(this))
            {
                break;
            }

            MoveOneStep();

            yield return new WaitForSeconds(0.05f);
        }

        isMoving = false;
    }

    private void MoveOneStep()
    {
        Vector3 nextHead = HeadCell + HeadDirection.ToVector();

        Vector3 tail = Cells[0];

        GridNode tailNode = gridManager.GetNode(tail);

        if (tailNode != null)
        {
            tailNode.OccupiedBlock = null;
        }

        Cells.RemoveAt(0);

        Cells.Add(nextHead);

        GridNode headNode = gridManager.GetNode(nextHead);

        if (headNode != null)
        {
            headNode.OccupiedBlock = this;
        }

        RefreshVisual();
    }

    public void RefreshVisual()
    {
        lineRenderer.positionCount = Cells.Count;


        for (int i = 0; i < Cells.Count; i++)
        {
            lineRenderer.SetPosition( i, gridManager.GridToWorld(Cells[i]));
        }

        UpdateHead();
    }

    private void UpdateHead()
    {
        headVisual.transform.position =  gridManager.GridToWorld(HeadCell);

        headVisual.transform.rotation =
            Quaternion.Euler(
                0,
                0,
                GetHeadAngle());
    }

    private float GetHeadAngle()
    {
        return HeadDirection switch
        {
            Direction.Up => 90f,
            Direction.Left => 180f,
            Direction.Down => -90f,
            _ => 0f
        };
    }

    //실제 이동 애매니메이션 -- 삭제예정
    public void ExitGrid()
    {
        gridManager.RemoveBlock(this);

        gameObject.SetActive(false);
    }



    public void Restore(List<Vector3> cells, Direction direction)
    {
        transform.DOKill();

        if (!StageManager.instance.ArrowBlocks.Contains(this))
                StageManager.instance.ArrowBlocks.Add(this);

        if (moveRoutine != null)
        {
            StopCoroutine(moveRoutine);
            moveRoutine = null;
        }

        gridManager.UnregisterBlock(this);

        gameObject.SetActive(true);

        Cells = new List<Vector3>(cells);

        HeadDirection = direction;

        gridManager.RegisterBlock(this);

        isMoving = false;

        RefreshVisual();
    }



    //막힘 애니메이션
    private void PlayBlockedAnimation()
    {
        //Vector3 dir = Direction.ToVector() * 0.15f;

        //transform .DOPunchPosition(
        //        dir,
        //        0.2f,
        //        5,
        //        0.5f);
    }



    //플레이 클릭 애니메이션
    public void PlayClickAnimation()
    {
        transform
            .DOScale(0.85f, 0.05f)
            .OnComplete(() =>
            {
                transform.DOScale(1f, 0.05f);
            });
    }




}