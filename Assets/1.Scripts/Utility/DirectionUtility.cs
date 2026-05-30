using UnityEngine;

public static class DirectionUtility
{
    public static Vector3 ToVector(this Direction dir)
    {
        return dir switch
        {
            Direction.Up => Vector3.up,
            Direction.Down => Vector3.down,
            Direction.Left => Vector3.left,
            Direction.Right => Vector3.right,
            _ => Vector3.zero
        };

    }

    // 2D 전용 쿼터니언 변환 메서드
    public static Quaternion ToRotation2D(this Direction dir)
    {
        Vector3 v = dir.ToVector();
        if (v == Vector3.zero) return Quaternion.identity;

        // Atan2로 라디안 각도를 구한 뒤 도(Degree) 단위로 변환
        float angle = Mathf.Atan2(v.y, v.x) * Mathf.Rad2Deg;

        // 2D는 오직 Z축 기준으로만 회전해야 합니다!
        return Quaternion.Euler(0, 0, angle);
    }


}