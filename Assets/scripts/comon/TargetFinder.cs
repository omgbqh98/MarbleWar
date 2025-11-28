using System;
using UnityEngine;

public static class TargetFinder
{
    // Tìm unit địch gần nhất. Trả về GameObject (unit.gameObject) hoặc null.
    // `origin` là vị trí kiểm tra; `myBase` để phân biệt team; `maxDistance` optional để giới hạn search.
    public static GameObject FindNearestEnemyUnit(Vector3 origin, Base myBase, float maxDistance = Mathf.Infinity)
    {
        if (myBase == null) return null;

        Unit[] allUnits = UnityEngine.Object.FindObjectsOfType<Unit>();
        GameObject nearest = null;
        float minDist = maxDistance;

        foreach (var u in allUnits)
        {
            if (u == null || u.Base == null) continue;
            if (u.Base.teamID == myBase.teamID) continue; // cùng team -> skip

            float d = Vector3.Distance(origin, u.transform.position);
            if (d < minDist)
            {
                minDist = d;
                nearest = u.gameObject;
            }
        }

        return nearest;
    }

    // Tìm base địch gần nhất
    public static GameObject FindNearestEnemyBase(Vector3 origin, Base myBase, float maxDistance = Mathf.Infinity)
    {
        if (myBase == null) return null;

        Base[] allBases = UnityEngine.Object.FindObjectsOfType<Base>();
        GameObject nearest = null;
        float minDist = maxDistance;

        foreach (var b in allBases)
        {
            if (b == null || b == myBase) continue;
            if (b.teamID == myBase.teamID) continue;

            float d = Vector3.Distance(origin, b.transform.position);
            if (d < minDist)
            {
                minDist = d;
                nearest = b.gameObject;
            }
        }

        return nearest;
    }

    public static GameObject FindNearestEnemySquare(Vector3 origin, Base myBase, float maxDistance = Mathf.Infinity)
    {
        if (myBase == null) return null;

        Square[] allSquares = UnityEngine.Object.FindObjectsOfType<Square>();
        GameObject nearest = null;
        float minDist = maxDistance;

        foreach (var u in allSquares)
        {
            if (u == null) continue;
            if (u.ownerBase != null && u.ownerBase.teamID == myBase.teamID) continue; // cùng team -> skip

            float d = Vector3.Distance(origin, u.transform.position);
            if (d < minDist)
            {
                minDist = d;
                nearest = u.gameObject;
            }
        }

        return nearest;
    }

    // Tìm target gần nhất giữa Unit và Base. Trả về object (unit OR base)
    // Nếu bạn muốn dùng threshold để ưu tiên unit khi tương đương, truyền preferUnitDelta (ví dụ 0.1f)
    public static GameObject FindNearestTarget(Vector3 origin, Base myBase, float maxDistance = Mathf.Infinity, float preferUnitDelta = 0f)
    {
        var unit = FindNearestEnemyUnit(origin, myBase, maxDistance);
        var baseObj = FindNearestEnemyBase(origin, myBase, maxDistance);

        if (unit == null) return baseObj;
        if (baseObj == null) return unit;

        float dUnit = Vector3.Distance(origin, unit.transform.position);
        float dBase = Vector3.Distance(origin, baseObj.transform.position);

        // nếu muốn ưu tiên unit khi khoảng cách gần bằng nhau: dUnit <= dBase + preferUnitDelta
        return (dUnit <= dBase + preferUnitDelta) ? unit : baseObj;
    }
}
