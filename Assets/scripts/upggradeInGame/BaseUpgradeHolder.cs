using System.Collections.Generic;
using UnityEngine;

public class BaseUpgradeHolder : MonoBehaviour
{
    // lưu số sao cho mỗi UpgradeSO
    private Dictionary<UpgradeSO, int> starLevels = new Dictionary<UpgradeSO, int>();

    // public read-only danh sách để UI sử dụng
    public List<UpgradeSO> appliedUpgrades = new List<UpgradeSO>();

    // trả về sao hiện tại (0 nếu chưa có)
    public int GetStarLevel(UpgradeSO up)
    {
        if (up == null) return 0;
        if (starLevels.TryGetValue(up, out int v)) return v;
        return 0;
    }

    // Add star, trả về số thực tế đã thêm (ví dụ 1) — không vượt quá maxStars
    public int AddStar(UpgradeSO up, int amount = 1)
    {
        if (up == null || amount <= 0) return 0;

        int current = GetStarLevel(up);
        int max = (up.maxStars <= 0) ? 5 : up.maxStars;
        if (current >= max) return 0;

        int canAdd = Mathf.Min(amount, max - current);
        int newVal = current + canAdd;
        starLevels[up] = newVal;

        if (!appliedUpgrades.Contains(up))
            appliedUpgrades.Add(up);

        return canAdd;
    }

    // (Tùy) method để set/reset, remove, etc.
    public void RemoveUpgrade(UpgradeSO up)
    {
        if (up == null) return;
        starLevels.Remove(up);
        appliedUpgrades.Remove(up);
    }
}
