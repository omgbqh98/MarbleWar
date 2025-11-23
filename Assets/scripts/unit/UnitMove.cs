using UnityEngine;

public class UnitMove : MonoBehaviour
{
    [Header("Thông số tìm kiếm mục tiêu")]
    private Base myBase; // 🔹 Base của unit này
    private UnitStats unitStats; // 🔹 Base của unit này
    private float rangeDefault = 1f;

    private void Start()
    {
        unitStats = GetComponent<UnitStats>();
        rangeDefault = unitStats.attackRange;
        // Nếu chưa gán thủ công, thử tìm tự động
        if (myBase == null)
            myBase = GetComponent<Unit>().Base;
    }

    private void Update()
    {
        GameObject target = FindNearestEnemy();
        if (target != null)
            MoveUnit(target);
    }

    // 🔹 Tìm Unit gần nhất nhưng khác Base
    GameObject FindNearestEnemy()
    {
        Unit[] allUnits = FindObjectsOfType<Unit>();
        GameObject nearest = null;
        float minDist = Mathf.Infinity;

        foreach (Unit other in allUnits)
        {
            // Bỏ qua nếu cùng Base hoặc null
            if (other == null || other.Base == null || other.Base == myBase)
                continue;

            if (other.Base.teamID == myBase.teamID) continue;

            float dist = Vector3.Distance(transform.position, other.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                nearest = other.gameObject;
            }
        }

        return nearest;
    }

    void MoveUnit(GameObject target)
    {
        float distanceToTarget = Vector3.Distance(transform.position, target.transform.position);

        if (distanceToTarget < rangeDefault)
        {
            return;
        }


        Vector3 direction = (target.transform.position - transform.position).normalized;
        transform.position += direction * unitStats.moveSpeed * Time.deltaTime;
    }
}
