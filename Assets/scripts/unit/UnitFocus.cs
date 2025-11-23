using UnityEngine;

public class UnitFocus : MonoBehaviour
{
    [Header("Refs")]
    private Base myBase;             // Base hiện tại của unit (để xác định team)
    public float rotationSpeed = 5f;

    [Header("Tùy chỉnh góc quay")]
    [Tooltip("Độ lệch góc (theo độ). Dương = quay sang phải, Âm = quay sang trái.")]
    public float rotationOffset = 0f;

    void Start()
    {
        // Nếu chưa gán, tự tìm base cha
        if (myBase == null)
            myBase = GetComponent<Unit>().Base;
    }

    void Update()
    {
        GameObject target = FindNearestEnemy();
        if (target != null)
        {
            RotateTowards(target);
        }
    }

    // 🔹 Tìm địch gần nhất (giống UnitMove)
    GameObject FindNearestEnemy()
    {
        Unit[] allUnits = FindObjectsOfType<Unit>();
        GameObject nearest = null;
        float minDist = Mathf.Infinity;

        foreach (Unit other in allUnits)
        {
            if (other == null || other.Base == null) continue;
            if (other.Base == myBase) continue;                    // cùng Base
            if (other.Base.teamID == myBase.teamID) continue;      // cùng team

            float dist = Vector2.Distance(transform.position, other.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                nearest = other.gameObject;
            }
        }

        return nearest;
    }

    // 🔹 Quay unit về phía kẻ địch
    void RotateTowards(GameObject enemy)
    {
        if (enemy == null) return;

        Vector2 dir = (enemy.transform.position - transform.position).normalized;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        angle += rotationOffset;

        // Quay dần
        Quaternion targetRot = Quaternion.Euler(0, 0, angle);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
    }
}
