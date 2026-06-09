using DG.Tweening;
using UnityEngine;

public abstract class BaseUI : MonoBehaviour
{
    public virtual void Open()
    {
        gameObject.SetActive(true);
    }

    public virtual void Close()
    {
        gameObject.SetActive(false);
    }

    public void OnClickAnimation()
    {
        transform.DOPunchScale(
            Vector3.one * 0.15f,
            0.15f);
    }

}
