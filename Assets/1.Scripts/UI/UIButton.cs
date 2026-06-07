using DG.Tweening;
using UnityEngine;

public class UIButton : MonoBehaviour
{
    public void OnClickAnimation()
    {
        transform.DOPunchScale(
            Vector3.one * 0.15f,
            0.15f);
    }
}
