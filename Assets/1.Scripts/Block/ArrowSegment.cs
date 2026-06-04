using UnityEngine;

public class ArrowSegment : MonoBehaviour
{
    public ArrowBlock Owner { get; private set; }

    public void Init(ArrowBlock owner)
    {
        Owner = owner;
    }

}
