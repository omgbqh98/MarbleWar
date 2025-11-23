using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class UpgradeStarEntry
{
    public UpgradeSO upgrade;
    public int stars;
}

public class BaseUpgradeHolder : MonoBehaviour
{
    [Tooltip("Danh sách upgrade + số sao đã được base này nhận")]
    public List<UpgradeStarEntry> upgradeStars = new List<UpgradeStarEntry>();

    [Tooltip("Danh sách upgrade đã được áp dụng (dùng để UI hiển thị & spawn unit)")]
    public List<UpgradeSO> appliedUpgrades = new List<UpgradeSO>();

    /// <summary>
    /// Trả về số sao hiện tại của upgrade (0 nếu chưa có).
    /// </summary>
    public int GetStarLevel(UpgradeSO up)
    {
        if (up == null) return 0;
        var entry = upgradeStars.Find(e => e.upgrade == up);
        return entry != null ? Mathf.Max(0, entry.stars) : 0;
    }

    /// <summary>
    /// Tăng sao cho upgrade. Nếu chưa có entry thì tạo mới.
    /// Trả về số sao thực sự được thêm.
    /// </summary>
    public int AddStar(UpgradeSO up, int amount = 1)
    {
        if (up == null) return 0;

        int curr = GetStarLevel(up);
        int max = (up.maxStars <= 0) ? 5 : up.maxStars;

        int toAdd = Mathf.Min(amount, max - curr);
        if (toAdd <= 0) return 0;

        // tìm theo tên để tránh reference mismatch
        var entry = upgradeStars.Find(e => e.upgrade != null && e.upgrade.name == up.name);

        if (entry == null)
        {
            entry = new UpgradeStarEntry { upgrade = up, stars = curr + toAdd };
            upgradeStars.Add(entry);
        }
        else
        {
            entry.stars = curr + toAdd;
            entry.upgrade = up; // ghi đè để đảm bảo reference khớp asset
        }

        if (!appliedUpgrades.Any(x => x.name == up.name))
            appliedUpgrades.Add(up);

        return toAdd;
    }


    /// <summary>
    /// Set số sao trực tiếp (ít dùng).
    /// </summary>
    public void SetStars(UpgradeSO up, int stars)
    {
        if (up == null) return;
        stars = Mathf.Max(0, stars);

        var entry = upgradeStars.Find(e => e.upgrade == up);

        if (entry == null)
        {
            entry = new UpgradeStarEntry { upgrade = up, stars = stars };
            upgradeStars.Add(entry);
        }
        else
        {
            entry.stars = stars;
        }

        if (!appliedUpgrades.Contains(up))
            appliedUpgrades.Add(up);
    }

    /// <summary>
    /// Xóa upgrade khỏi holder.
    /// </summary>
    public void RemoveUpgrade(UpgradeSO up)
    {
        if (up == null) return;
        upgradeStars.RemoveAll(e => e.upgrade == up);
        appliedUpgrades.Remove(up);
    }
}
