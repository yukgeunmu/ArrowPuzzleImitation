using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class ArrowBlock : BlockBase
{
    [SerializeField]
    private LineRenderer lineRenderer;
    private Coroutine clickAnimaRoutine;

    [SerializeField]
    private Transform headVisual;

    [SerializeField]
    private EdgeCollider2D edgeCollider;


    [Header("두께 설정")]
    [SerializeField] private float normalWidth = 0.25f;    // 평소 두께
    [SerializeField] private float targetWidth = 0.1f;    // 클릭했을 때 최대 두께

    [Header("애니메이션 속도")]
    [SerializeField] private float shrinkDuration = 0.3f; // 원래대로 돌아오는 데 걸리는 시간

    [Header("화살이동 속도")]
    [SerializeField] private float moveVelocity = 0.05f;


    public int Id;

    public List<Vector3> Cells { get; private set; }

    public Direction HeadDirection;

    public Vector3 HeadCell => Cells[Cells.Count - 1];
   

    private Coroutine moveRoutine;

    private bool isMoving;


    public void Init(List<Vector3> cells,Direction headDirection, int Id)
    {
        if(Cells != null)
            ResetState();

        Cells = new List<Vector3>(cells);

        this.HeadDirection = headDirection;

        this.Id = Id;

        this.lineRenderer.startWidth = normalWidth;
        this.lineRenderer.endWidth = normalWidth;
        this.lineRenderer.startColor = Color.black;
        this.lineRenderer.endColor = Color.black;
        this.headVisual.transform.localScale = Vector3.one;

        RefreshVisual();
    }


    public void TryMove()
    {
        PlayClickAnimation();

        if (isMoving)
            return;


        if (!Manager.Instance.Grid.CanMoveShape(this))
        {
            Manager.Instance.Sound.Play(SFXType.Blocked);
            PlayBlockedAnimation();
            return;
        }

        Manager.Instance.Sound.Play(SFXType.Move);
        Manager.Instance.Undo.Execute(new MoveCommand(this));
           
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

            if (Manager.Instance.Grid.IsCompletelyOut(this))
            {
                ExitGrid();
                break;
            }

            if (!Manager.Instance.Grid.CanMoveShape(this))
            {
                break;
            }

            MoveOneStep();

            yield return new WaitForSeconds(moveVelocity);
        }

        isMoving = false;
    }

    private void MoveOneStep()
    {
        Vector3 nextHead = HeadCell + HeadDirection.ToVector();

        Vector3 tail = Cells[0];

        GridNode tailNode = Manager.Instance.Grid.GetNode(tail);

        if (tailNode != null)
        {
            tailNode.OccupiedBlock = null;
        }

        Cells.RemoveAt(0);

        Cells.Add(nextHead);

        GridNode headNode = Manager.Instance.Grid.GetNode(nextHead);

        if (headNode != null)
        {
            headNode.OccupiedBlock = this;
        }

        RefreshVisual();
    }

    public void RefreshVisual()
    {
        lineRenderer.positionCount = Cells.Count;
        List<Vector2> edgePoints = new List<Vector2>();

        for (int i = 0; i < Cells.Count; i++)
        {
            lineRenderer.SetPosition( i, Cells[i]);
            edgePoints.Add(Cells[i]);
        }

        edgeCollider.SetPoints(edgePoints);
        
        UpdateHead();
    }

    private void UpdateHead()
    {
        headVisual.transform.position =  Manager.Instance.Grid.GridToWorld(HeadCell);

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



    public void ExitGrid()
    {
        Manager.Instance.Grid.RemoveBlock(this);

        gameObject.SetActive(false);
    }



    public void Restore(List<Vector3> cells, Direction direction)
    {
        transform.DOKill();

        if (!Manager.Instance.Stage.ArrowBlocks.Contains(this))
                Manager.Instance.Stage.ArrowBlocks.Add(this);

        if (moveRoutine != null)
        {
            StopCoroutine(moveRoutine);
            moveRoutine = null;
        }

        Manager.Instance.Grid.UnregisterBlock(this);

        gameObject.SetActive(true);

        Cells = new List<Vector3>(cells);

        HeadDirection = direction;

        Manager.Instance.Grid.RegisterBlock(this);

        isMoving = false;

        RefreshVisual();
    }



    //막힘 애니메이션
    private void PlayBlockedAnimation()
    {
        transform.DOKill();

        transform.DOShakePosition(
            0.15f,
            0.15f,
            10,
            90f,
            false,
            true);
    }



    //플레이 클릭 애니메이션
    public void PlayClickAnimation()
    {
        if (clickAnimaRoutine != null)
        {
            StopCoroutine(clickAnimaRoutine);
        }


        clickAnimaRoutine = StartCoroutine(PlayLineAnimation());

    }

    IEnumerator PlayLineAnimation()
    {
        lineRenderer.startWidth = targetWidth;
        lineRenderer.endWidth = targetWidth;

        float percent = targetWidth / normalWidth;

        float timeElapsed = 0f;

        while (timeElapsed < shrinkDuration)
        {
            timeElapsed += Time.deltaTime;
            float progress = timeElapsed / shrinkDuration;

            float currentWidth = Mathf.Lerp(targetWidth, normalWidth, progress);
            float headScale = Mathf.Lerp(percent, 1, progress);

            lineRenderer.startWidth = currentWidth;
            lineRenderer.endWidth = currentWidth;

            headVisual.transform.localScale = new Vector3(headScale, headScale, 1);

            yield return null; // 다음 프레임까지 대기
        }

        // 3. 완벽하게 원래 두께로 고정
        lineRenderer.startWidth = normalWidth;
        lineRenderer.endWidth = normalWidth;

    }

    public void PlayHint()
    {
        StartCoroutine(HintRoutine());
    }

    private IEnumerator HintRoutine()
    {
        Color origin = lineRenderer.startColor;

        for (int i = 0; i < 3; i++)
        {
            lineRenderer.startColor = Color.yellow;

            lineRenderer.endColor = Color.yellow;

            yield return
                new WaitForSeconds(
                    0.2f);

            lineRenderer.startColor = origin;

            lineRenderer.endColor = origin;

            yield return new WaitForSeconds(0.2f);
        }
    }

    public void ResetState()
    {
        transform.DOKill();

        Cells.Clear();

        isMoving = false;
    }
}