// UpgradeManager.cs
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager Instance;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    [Header("Settings")]
    public int choicesCount = 3;
    public List<UpgradeSO> globalUpgrades = new List<UpgradeSO>(); // optional global pool

    // Lấy upgrades từ list prefab (không cần spawn)
    public List<UpgradeSO> GatherUpgradesFromPrefabs(List<GameObject> unitPrefabs)
    {
        List<UpgradeSO> result = new List<UpgradeSO>();
        if (globalUpgrades != null) result.AddRange(globalUpgrades);
        if (unitPrefabs == null) return result;
        foreach (var p in unitPrefabs)
        {
            if (p == null) continue;
            var pool = p.GetComponent<UnitUpgradePool>();
            if (pool != null && pool.upgrades != null)
                result.AddRange(pool.upgrades);
        }
        // loại bỏ trùng reference
        return result.Distinct().ToList();
    }

    // Trả n lựa chọn đã shuffle
    public List<UpgradeSO> GetRandomChoicesFromPrefabs(List<GameObject> prefabs, int n)
    {
        var all = GatherUpgradesFromPrefabs(prefabs);
        if (all.Count <= n) return new List<UpgradeSO>(all);

        // Fisher-Yates shuffle
        for (int i = 0; i < all.Count; i++)
        {
            int j = Random.Range(i, all.Count);
            var tmp = all[i];
            all[i] = all[j];
            all[j] = tmp;
        }
        return all.Take(n).ToList();
    }

    public List<UpgradeSO> GetRandomChoicesForBase(Base ownerBase, List<GameObject> prefabs, int n)
    {
        var all = GatherUpgradesFromPrefabs(prefabs);

        var holder = ownerBase != null
            ? ownerBase.GetComponent<BaseUpgradeHolder>()
            : null;

        List<UpgradeSO> eligible = new List<UpgradeSO>();

        foreach (var up in all)
        {
            if (up == null) continue;

            int maxStars = (up.maxStars <= 0) ? 5 : up.maxStars;
            int currentStars = holder != null ? holder.GetStarLevel(up) : 0;

            // Chỉ cho phép upgrade chưa max sao
            if (currentStars < maxStars)
            {
                eligible.Add(up);
            }
        }

        if (eligible.Count <= n) return new List<UpgradeSO>(eligible);

        // Shuffle
        for (int i = 0; i < eligible.Count; i++)
        {
            int j = Random.Range(i, eligible.Count);
            var tmp = eligible[i];
            eligible[i] = eligible[j];
            eligible[j] = tmp;
        }

        return eligible.GetRange(0, n);
    }


    // Áp dụng upgrade cho 1 Base: hiện có + save để apply cho spawns tương lai (nếu cần)
    public void ApplyUpgradeToBase(Base targetBase, UpgradeSO upgrade, bool applyToFutureSpawns = true)
    {
        if (targetBase == null)
        {
            Debug.LogWarning("[UpgradeManager] ApplyUpgradeToBase: targetBase is null.");
            return;
        }
        if (upgrade == null)
        {
            Debug.LogWarning("[UpgradeManager] ApplyUpgradeToBase: upgrade is null.");
            return;
        }

        // ensure holder exists if we want to persist stars
        var holder = targetBase.GetComponent<BaseUpgradeHolder>();
        if (holder == null && applyToFutureSpawns)
        {
            holder = targetBase.gameObject.AddComponent<BaseUpgradeHolder>();
        }

        int currentStars = (holder != null) ? holder.GetStarLevel(upgrade) : 0;
        int maxStars = (upgrade.maxStars <= 0) ? 5 : upgrade.maxStars;

        // nếu đã max thì bỏ
        if (currentStars >= maxStars)
        {
            Debug.Log($"[UpgradeManager] Upgrade '{upgrade.title}' already at max stars ({currentStars}).");
            return;
        }

        // Nếu chúng ta muốn persist cho future spawns thì tăng sao trên holder
        int addedStars = 0;
        if (applyToFutureSpawns)
        {
            // giả sử AddStar trả về số sao thực sự được thêm (ví dụ 1)
            addedStars = holder.AddStar(upgrade); // bạn nên implement AddStar để return int
        }
        else
        {
            // nếu không lưu cho tương lai thì ta vẫn muốn ứng xử như đã tăng 1 sao nhưng không ghi vào holder
            addedStars = 1;
        }

        // bảo đảm không vượt max
        if (currentStars + addedStars > maxStars)
            addedStars = maxStars - currentStars;

        if (addedStars <= 0)
        {
            return;
        }

        // 1) Apply modifier 'addedStars' lần cho tất cả unit hiện có (nếu upgrade là incremental per star)
        //    Nếu modifier thiết kế là "mỗi sao = thêm 1 lần modifier", ta apply addedStars lần.
        //    Nếu bạn muốn khác (ví dụ mỗi sao cộng phần trăm tổng hợp), đổi logic ApplyModifier tương ứng.
        foreach (var u in targetBase.Units)
        {
            if (u == null) continue;
            var stats = u.GetComponent<UnitStats>();
            if (stats == null) continue;

            if (upgrade.applyToAllUnitTypes || stats.unitType == upgrade.targetUnitType)
            {
                // apply as many times as addedStars (mỗi lần thể hiện 1 'star')
                for (int k = 0; k < addedStars; k++)
                {
                    stats.ApplyModifier(upgrade.modifier);
                }
            }
        }

        // 2) Nếu bạn lưu stars trong holder và muốn UI cập nhật ngay:
        if (SelectedListPanel.Instance != null)
        {
            SelectedListPanel.Instance.RefreshForBase(targetBase);
        }
    }


}
