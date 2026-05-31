using UnityEngine;

public abstract class BlockBase : MonoBehaviour
{
    public Vector3 GridPos;

    public virtual bool IsMovable => false;
}