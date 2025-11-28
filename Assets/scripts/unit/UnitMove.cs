using UnityEngine;

public class UnitMove : MonoBehaviour
{
    [Header("Avoidance")]
    public float avoidDistance = 0.5f;       // khoảng cách kiểm tra sang hai bên
    public float avoidCheckDistance = 1.0f;  // khoảng kiểm tra phía trước
    public LayerMask wallMask;

    private Base myBase;
    private UnitStats unitStats;
    private float rangeDefault; // attack range

    private void Start()
    {
        unitStats = GetComponent<UnitStats>();
        if (unitStats == null)
        {
            Debug.LogWarning($"{name}: UnitStats missing!");
            unitStats = new UnitStats(); // fallback (không ideal)
        }

        rangeDefault = unitStats.attackRange;

        if (myBase == null)
            myBase = GetComponent<Unit>().Base;

        // nếu object tạo runtime muốn auto gán wall layer (tuỳ)
        if (wallMask == 0)
            wallMask = LayerMask.GetMask("Wall");
    }

    private void Update()
    {
        GameObject target = TargetFinder.FindNearestTarget(transform.position, myBase, Mathf.Infinity, 0.05f);

        if (unitStats.unitType == UnitType.Worker)
        {
            var sq = TargetFinder.FindNearestEnemySquare(transform.position, myBase, Mathf.Infinity);
            if (sq != null) target = sq;
        }

        if (target != null)
            MoveUnit(target);
    }

    //---------------------------------------------
    // MOVE UNIT + TRÁNH TƯỜNG (và hành vi Archer lùi)
    //---------------------------------------------
    void MoveUnit(GameObject target)
    {
        if (target == null) return;

        float dist = Vector3.Distance(transform.position, target.transform.position);

        // Giới hạn tầm archer
        float minRange = rangeDefault * 0.6f;   // quá gần → lùi
        float maxRange = rangeDefault;  // quá xa → tiến lại gần
                                        // Bạn có thể chỉnh 0.7f và 1.05f nếu muốn

        // --- SPECIAL: Archer giữ khoảng cách tối ưu ---
        if (unitStats.unitType == UnitType.Archer)
        {
            // 1) Địch QUÁ GẦN → LÙI LẠI
            if (dist < minRange)
            {
                Vector3 away = (transform.position - target.transform.position).normalized;
                Vector3 moveDir = GetAvoidanceDirection(away);

                float retreatSpeed = unitStats.moveSpeed * 0.5f; // 🔥 chậm hơn 50%
                transform.position += moveDir * retreatSpeed * Time.deltaTime;
                return;
            }

            // 2) Địch QUÁ XA → TIẾN LÊN
            if (dist > maxRange)
            {
                Vector3 toTarget = (target.transform.position - transform.position).normalized;
                Vector3 moveDir = GetAvoidanceDirection(toTarget);
                transform.position += moveDir * unitStats.moveSpeed * Time.deltaTime;
                return;
            }

            // 3) Nằm trong tầm tối ưu → đứng yên (bắn)
            return;
        }

        // --- Default behavior cho các unit khác ---
        if (dist < rangeDefault)
            return;

        Vector3 dirToTarget = (target.transform.position - transform.position).normalized;
        Vector3 finalDir = GetAvoidanceDirection(dirToTarget);

        transform.position += finalDir * unitStats.moveSpeed * Time.deltaTime;
    }



    //---------------------------------------------
    // HÀM NÀY CHỈ XỬ LÝ TRÁNH TƯỜNG
    // Trả về hướng di chuyển (có thể là dir gốc, left/right, hoặc -dir nếu kẹt)
    //---------------------------------------------
    Vector3 GetAvoidanceDirection(Vector3 dir)
    {
        // Nếu không set wallMask -> không kiểm tra
        if (wallMask == 0)
            return dir;

        // Kiểm tra phía trước có tường không
        bool blocked = Physics2D.Raycast(transform.position, dir, avoidCheckDistance, wallMask);

        if (!blocked)
            return dir; // Không bị chắn → đi thẳng / lui thẳng

        // Nếu bị chắn → thử né trái & phải
        Vector3 left = new Vector3(-dir.y, dir.x);
        Vector3 right = new Vector3(dir.y, -dir.x);

        bool leftBlocked = Physics2D.Raycast(transform.position, left, avoidDistance, wallMask);
        bool rightBlocked = Physics2D.Raycast(transform.position, right, avoidDistance, wallMask);

        // Né bên trái nếu phải bị chắn
        if (!leftBlocked && rightBlocked)
            return left;

        // Né bên phải nếu trái bị chắn
        if (!rightBlocked && leftBlocked)
            return right;

        // Cả hai đều trống -> chọn bên đưa bạn gần hướng gốc hơn
        if (!leftBlocked && !rightBlocked)
        {
            Vector3 leftProbe = transform.position + left * avoidDistance;
            Vector3 rightProbe = transform.position + right * avoidDistance;

            float distLeft = Vector3.Distance(leftProbe, transform.position + dir);
            float distRight = Vector3.Distance(rightProbe, transform.position + dir);

            return distLeft <= distRight ? left : right;
        }

        // Cả hai bên đều bị block -> lùi lại (đổi hướng)
        return -dir;
    }
}
