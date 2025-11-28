using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Transform))]
public class GroupMove : MonoBehaviour
{
    [Header("Avoidance (group center)")]
    public float avoidDistance = 0.5f;        // khoảng cách lách tường (kiểm tra bên)
    public float avoidCheckDistance = 1.0f;   // khoảng để raycast kiểm tra phía trước
    public LayerMask wallMask;                // gán layer "Wall" trong Inspector

    [Header("Thông số di chuyển")]
    public float moveSpeed = 0.5f;            // Tốc độ di chuyển của cả nhóm (center)
    public float separationDistance = 1f;     // Khoảng cách để tách đội hình và cho units tự chạy riêng

    // cache
    private Base myBase;                      // Base hiện tại của group (để xác định team)
    private UnitStats sampleUnitStats;        // dùng để lấy default nếu cần
    private Transform groupTransform;

    void Start()
    {
        wallMask = LayerMask.GetMask("Wall");
        groupTransform = transform;
        myBase = GetComponentInParent<Base>();

        // Try to get a sample UnitStats from any child for sensible defaults
        var stat = GetComponentInChildren<UnitStats>();
        if (stat != null)
        {
            sampleUnitStats = stat;
            // nếu separationDistance chưa được tùy chỉnh (<=0) thì lấy từ sample unit
            if (separationDistance <= 0f)
                separationDistance = sampleUnitStats.attackRange;
        }
    }

    private void Update()
    {
        // Tìm target gần nhất (unit hoặc base, hoặc square nếu unit type là Worker)
        GameObject target = TargetFinder.FindNearestTarget(groupTransform.position, myBase, Mathf.Infinity, 0.05f);

        // Nếu không tìm thấy target, không di chuyển
        if (target == null) return;

        // Nếu sampleUnitStats tồn tại và là Worker, ưu tiên square (một nhóm Worker thì có ý nghĩa)
        if (sampleUnitStats != null && sampleUnitStats.unitType == UnitType.Worker)
        {
            var sq = TargetFinder.FindNearestEnemySquare(groupTransform.position, myBase, Mathf.Infinity);
            if (sq != null) target = sq;
        }

        MoveUnit(target);
    }

    // Di chuyển nhóm (center) về phía mục tiêu (với avoidance đơn giản)
    void MoveUnit(GameObject target)
    {
        if (target == null) return;

        float distanceToTarget = Vector3.Distance(groupTransform.position, target.transform.position);

        // Nếu đã vào tầm để tách đội hình -> bật UnitMove trên từng unit con và tắt GroupMove
        if (distanceToTarget < separationDistance)
        {
            EnableChildrenUnitMove();
            // disable this GroupMove component so units take over movement
            this.enabled = false;
            return;
        }

        // Tính hướng đến mục tiêu
        Vector3 direction = (target.transform.position - groupTransform.position).normalized;

        // Sử dụng hàm tránh tường (trả về hướng di chuyển thực tế)
        Vector3 moveDir = GetAvoidanceDirection(direction);

        // Di chuyển group center
        groupTransform.position += moveDir.normalized * moveSpeed * Time.deltaTime;
    }

    // Bật component UnitMove trên các unit con (nếu có)
    void EnableChildrenUnitMove()
    {
        foreach (Transform child in transform)
        {
            if (child == null) continue;
            var unitMove = child.GetComponent<UnitMove>();
            if (unitMove != null)
            {
                unitMove.enabled = true;
            }
        }
    }

    // HÀM RIÊNG: xử lý tránh tường cho group center
    Vector3 GetAvoidanceDirection(Vector3 dir)
    {
        // nếu không có wallMask thì không check
        if (wallMask == 0)
            return dir;

        // Raycast phía trước để kiểm tra tường
        bool blocked = Physics2D.Raycast(groupTransform.position, dir, avoidCheckDistance, wallMask);

        if (!blocked)
            return dir;

        // Nếu bị chắn -> thử né trái / phải
        Vector3 left = new Vector3(-dir.y, dir.x);
        Vector3 right = new Vector3(dir.y, -dir.x);

        bool leftBlocked = Physics2D.Raycast(groupTransform.position, left, avoidDistance, wallMask);
        bool rightBlocked = Physics2D.Raycast(groupTransform.position, right, avoidDistance, wallMask);

        if (!leftBlocked && rightBlocked)
            return left;
        if (!rightBlocked && leftBlocked)
            return right;
        if (!leftBlocked && !rightBlocked)
        {
            EnableChildrenUnitMove();
            // cả hai đều trống: chọn hướng đưa center lại gần hướng gốc hơn
            Vector3 leftProbe = groupTransform.position + left * avoidDistance * 0.5f;
            Vector3 rightProbe = groupTransform.position + right * avoidDistance * 0.5f;
            float leftDist = Vector3.Distance(leftProbe, groupTransform.position + dir);
            float rightDist = Vector3.Distance(rightProbe, groupTransform.position + dir);
            return (leftDist <= rightDist) ? left : right;
        }

        // cả hai bên đều bị chặn -> lùi nhẹ
        return -dir;
    }

    // Vẽ gizmo để dễ debug trong Editor
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, separationDistance);

        // Vẽ raycast phía trước cho debug (Editor only)
#if UNITY_EDITOR
        if (Application.isPlaying == false) return;
        Gizmos.color = Color.yellow;
        Vector3 fwd = transform.right; // not exact target dir but helpful
        Gizmos.DrawLine(transform.position, transform.position + fwd * avoidCheckDistance);
#endif
    }
}
