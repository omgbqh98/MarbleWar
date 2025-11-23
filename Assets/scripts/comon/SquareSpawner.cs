using UnityEngine;

public class SquareSpawner : MonoBehaviour
{
    [Header("Prefab hình vuông")]
    public GameObject squarePrefab;

    [Header("Kích thước lưới")]
    public int rows = 5;   // số hàng
    public int cols = 8;   // số cột

    [Header("Khoảng cách giữa các ô")]
    public float spacing = 1.1f;  // điều chỉnh cho vừa khít prefab (1 = sát, >1 = có khoảng cách)

    void Start()
    {
        SpawnSquares();
    }

    void SpawnSquares()
    {
        if (squarePrefab == null)
        {
            Debug.LogError("Chưa gán prefab!");
            return;
        }

        // Tính offset để lưới nằm giữa (0,0)
        float offsetX = (cols - 1) * spacing * 0.5f;
        float offsetY = (rows - 1) * spacing * 0.5f;

        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < cols; x++)
            {
                Vector3 pos = new Vector3(x * spacing - offsetX, y * spacing - offsetY, 0);
                Instantiate(squarePrefab, pos, Quaternion.identity, transform);
            }
        }
    }
}
