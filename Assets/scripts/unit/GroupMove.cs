using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroupMove : MonoBehaviour
{
    // Update is called once per frame
    [Header("Thông số di chuyển")]
    public float moveSpeed = 0.5f; // Tốc độ di chuyển của quân lính
    public float separationDistance = 1f; // Khoảng cách để tách đội hình và di chuyển độc lập

    [Header("Thông số tìm kiếm mục tiêu")]
    public string targetTag = "Team2"; // Tag của mục tiêu (Team2)

    // Start is called before the first frame update
    void Start()
    {
    }
    private void Update()
    {
        // Kiểm tra mục tiêu gần nhất và di chuyển quân lính
        GameObject target = FindNearestTarget(targetTag);
        if (target != null)
        {
            MoveUnit(target);
        }
    }

    // Tìm mục tiêu gần nhất có tag nhất định
    GameObject FindNearestTarget(string tag)
    {
        GameObject[] targets = GameObject.FindGameObjectsWithTag(tag);
        GameObject nearestTarget = null;
        float minDistance = Mathf.Infinity;

        foreach (GameObject target in targets)
        {
            float distance = Vector3.Distance(transform.position, target.transform.position);
            if (distance < minDistance)
            {
                nearestTarget = target;
                minDistance = distance;
            }
        }

        return nearestTarget;
    }

    // Di chuyển quân lính về phía mục tiêu
    void MoveUnit(GameObject target)
    {
        // Tính khoảng cách giữa quân lính và mục tiêu
        float distanceToTarget = Vector3.Distance(transform.position, target.transform.position);

        // Kiểm tra nếu quân lính đã gần mục tiêu đủ để tách đội hình
        if (distanceToTarget < separationDistance)
        {
            foreach (Transform child in transform) // Duyệt qua tất cả quân lính con
            {
                UnitMove unitMovement = child.GetComponent<UnitMove>();
                if (unitMovement != null)
                {
                    unitMovement.enabled = true;
                }
            }
            GroupMove groupMove = GetComponent<GroupMove>();
            if (groupMove != null)
            {
                groupMove.enabled = false;
            }
        }

        // Di chuyển quân lính về phía mục tiêu
        Vector3 direction = (target.transform.position - transform.position).normalized;
        transform.position += direction * moveSpeed * Time.deltaTime;

    }

    // Vẽ gizmo để dễ dàng nhìn thấy separationDistance trong Editor
    private void OnDrawGizmos()
    {
        // Vẽ một vòng tròn xung quanh đối tượng để hiển thị separationDistance
        Gizmos.color = Color.green; // Chọn màu xanh để dễ nhìn
        Gizmos.DrawWireSphere(transform.position, separationDistance); // Vẽ vòng tròn xung quanh vị trí của quân lính
    }
}
