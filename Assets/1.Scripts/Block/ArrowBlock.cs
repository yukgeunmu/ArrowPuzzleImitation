using UnityEngine;
using DG.Tweening;

public class ArrowBlock : BlockBase
{
    public override bool IsMovable =>  true;

    public Direction Direction;

    private GridManager gridManager;

    public bool IsMoving { get; private set; }

    public void Init( Vector3 pos, Direction dir, GridManager manager)
    {
        GridPos = pos;
        Direction = dir;
        gridManager = manager;

        transform.rotation = DirectionUtility.ToRotation2D(dir);
    }

    public void TryMove()
    {
        if (IsMoving)
            return;

        if (!gridManager.CanMove(GridPos, Direction))
        {
            PlayBlockedAnimation();
            return;
        }

        UndoManager.Instance.Execute(new MoveCommand(this));
    }


    //실제 이동 애매니메이션
    public void ExitGrid()
    {
        IsMoving = true;

        gridManager.RemoveBlock(GridPos);

        Vector3 targetPos =
            transform.position + Direction.ToVector() * 10f;

        transform
            .DOMove(targetPos, 0.35f)
            .SetEase(Ease.OutQuad)
            .OnComplete(() =>
            {
                gameObject.SetActive(false);
            });
    }

    public void Restore(Vector3 position)
    {
        transform.DOKill();

        gameObject.SetActive(true);

        GridPos = position;

        transform.position =
            gridManager.GridToWorld(position);

        gridManager.RegisterBlock(position, this);

        IsMoving = false;
    }



    //막힘 애니메이션
    private void PlayBlockedAnimation()
    {
        Vector3 dir = Direction.ToVector() * 0.15f;

        transform .DOPunchPosition(
                dir,
                0.2f,
                5,
                0.5f);
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