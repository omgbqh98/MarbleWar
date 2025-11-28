using UnityEngine;

public class UnitFocus : MonoBehaviour
{
    [Header("Refs")]
    private Base myBase;             // Base hiện tại của unit (để xác định team)
    public float rotationSpeed = 5f;

    [Header("Tùy chỉnh góc quay")]
    [Tooltip("Độ lệch góc (theo độ). Dương = quay sang phải, Âm = quay sang trái.")]
    public float rotationOffset = 0f;
    private UnitStats unitStats;

    void Start()
    {
        unitStats = GetComponent<UnitStats>();
        // Nếu chưa gán, tự tìm base cha
        if (myBase == null)
            myBase = GetComponent<Unit>().Base;
    }

    void Update()
    {
        GameObject target = TargetFinder.FindNearestTarget(transform.position, myBase, Mathf.Infinity, 0.05f);
        if (unitStats.unitType == UnitType.Worker)
        {
            target = TargetFinder.FindNearestEnemySquare(transform.position, myBase, Mathf.Infinity);
        }
        if (target != null)
        {
            RotateTowards(target);
        }
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
