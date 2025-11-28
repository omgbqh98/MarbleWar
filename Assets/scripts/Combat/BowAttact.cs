using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BowAttact : MonoBehaviour
{
    [Header("Prefab mũi tên")]
    public GameObject arrowPrefab;          // Mũi tên prefab
    public GameObject arrowInit;          // Mũi tên prefab
    public Transform shootPoint;            // Vị trí bắn ra (giữa cây cung)
    public Vector3 customArrowPosition;

    [Header("Thông số bắn")]
    public float shootForce = 10f;          // Lực bắn mũi tên
    private GameObject currentArrow;        // Biến để lưu mũi tên hiện tại
    UnitStats unitStats;
    void Awake()
    {
        CreateArrow();
        unitStats = GetComponent<UnitStats>();
    }
    void Update()
    {
    }

    // Sinh mũi tên tại vị trí cây cung
    void CreateArrow()
    {
        // Tạo mũi tên tại vị trí của cây cung
        currentArrow = Instantiate(arrowInit, transform.position, arrowInit.transform.rotation);
        currentArrow.transform.SetParent(transform);
        currentArrow.transform.localPosition = customArrowPosition;
        currentArrow.transform.localRotation = arrowInit.transform.rotation;
    }

    // Bắn mũi tên ra
    public void ShootArrows()
    {
        // Sử dụng tỉ lệ bắn mũi tên (tối đa từ 1 đến 3 mũi tên)
        int arrowsToShoot = unitStats.bullet1Time;

        for (int i = 0; i < arrowsToShoot; i++)
        {
            if (currentArrow != null)
            {
                Destroy(currentArrow);
                currentArrow = null;
            }
            // Tạo mũi tên tại shootPoint
            GameObject arrow = Instantiate(arrowPrefab, shootPoint.position, shootPoint.rotation);
            arrow.transform.SetParent(transform);

            Vector2 direction = shootPoint.right;  // Hướng mũi tên đang đặt
            float[] angles = GenerateAngles(arrowsToShoot);

            if (arrowsToShoot == 1)
            {
                direction = Quaternion.Euler(0, 10, 0) * direction;
            }
            else
            {
                direction = Quaternion.Euler(0, 0, angles[i % angles.Length]) * direction;
            }

            // Di chuyển mũi tên
            Rigidbody2D rb = arrow.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.velocity = direction * shootForce; // Áp dụng lực bắn vào mũi tên
            }
        }

        StartCoroutine(ReloadArrow());
    }

    IEnumerator ReloadArrow()
    {
        yield return new WaitForSeconds(0.5f);
        CreateArrow();
    }

    private float[] GenerateAngles(int bulletNumber)
    {
        float[] angles = new float[bulletNumber];
        float step = 20.0f / (bulletNumber - 1); // Khoảng cách giữa các góc
        for (int i = 0; i < bulletNumber; i++)
        {
            angles[i] = -10 + step * i;
        }
        return angles;
    }
}
