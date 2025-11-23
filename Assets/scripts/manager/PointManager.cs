using System;
using UnityEngine;

[DefaultExecutionOrder(-95)]
public class PointManager : MonoBehaviour
{
    public static PointManager Instance { get; private set; }

    [Header("Points")]
    public int currentPoints;  // điểm đang còn
    public int maxPoints;      // trần điểm đã mua
    public int maxLimit = 50;

    [Header("Buy Config")]
    public int baseCost = 100;       // giá coin cho lần mua đầu
    public float costGrowth = 1.25f; // tăng dần giá mỗi point mua thêm

    public event Action OnPointsChanged;

    const string P_CUR = "POINTS_CUR";
    const string P_MAX = "POINTS_MAX";

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Lần đầu: 1/1
        maxPoints = PlayerPrefs.GetInt(P_MAX, 1);
        currentPoints = PlayerPrefs.GetInt(P_CUR, maxPoints); // mặc định = max ở lần đầu
        Save();
    }

    public int GetBuyCost()
    {
        // Chi phí mua point thứ (maxPoints) — ví dụ: 1/1 đang có → mua point thứ 2
        // Có thể dùng công thức base * growth^(maxPoints-1)
        int nth = Mathf.Max(1, maxPoints);
        return Mathf.RoundToInt(baseCost * Mathf.Pow(costGrowth, nth - 1));
    }

    public bool TryBuyPoint()
    {
        // ✅ Kiểm tra giới hạn trước
        if (maxPoints >= maxLimit)
            return false; // Không thể mua thêm

        int cost = GetBuyCost();
        if (!CurrencyManager.Instance.TrySpend(cost)) return false;

        maxPoints++;
        currentPoints++; // tăng song song
        Save();
        OnPointsChanged?.Invoke();
        return true;
    }

    public bool TrySpendPoint(int amount = 1)
    {
        if (currentPoints < amount) return false;
        currentPoints -= amount;
        Save();
        OnPointsChanged?.Invoke();
        return true;
    }

    public void RefundPoints(int amount)
    {
        currentPoints = Mathf.Min(currentPoints + amount, maxPoints);
        Save();
        OnPointsChanged?.Invoke();
    }

    public void Save()
    {
        PlayerPrefs.SetInt(P_CUR, currentPoints);
        PlayerPrefs.SetInt(P_MAX, maxPoints);
        PlayerPrefs.Save();
    }
}
