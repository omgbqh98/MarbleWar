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


    public void ApplyUpgradeToBase(Base targetBase, UpgradeSO upgrade, bool applyToFutureSpawns = true)
    {
        if (targetBase == null || upgrade == null)
        {
            Debug.LogWarning("[UpgradeManager] invalid args");
            return;
        }

        // Ensure holder exists if we want persistence
        BaseUpgradeHolder holder = null;
        if (applyToFutureSpawns)
        {
            holder = targetBase.GetComponent<BaseUpgradeHolder>();
            if (holder == null) holder = targetBase.gameObject.AddComponent<BaseUpgradeHolder>();
        }

        int currentStars = holder != null ? holder.GetStarLevel(upgrade) : 0;
        int maxStars = (upgrade.maxStars <= 0) ? 5 : upgrade.maxStars;
        if (currentStars >= maxStars)
        {
            return;
        }

        // Add star(s) to holder (if requested) — this returns actual number added
        int addedStars = 0;
        if (applyToFutureSpawns)
        {
            addedStars = holder.AddStar(upgrade, 1); // add 1 star by default
        }
        else
        {
            addedStars = 1;
        }

        if (addedStars <= 0)
        {
            return;
        }

        // Apply modifier to currently spawned units (apply addedStars times if design requires)
        int appliedCount = 0;
        foreach (var u in targetBase.Units)
        {
            if (u == null) continue;
            var stats = u.GetComponent<UnitStats>();
            if (stats == null) continue;
            if (upgrade.applyToAllUnitTypes || stats.unitType == upgrade.targetUnitType)
            {
                for (int k = 0; k < addedStars; k++)
                {
                    stats.ApplyModifier(upgrade.modifier);
                }
                appliedCount++;
            }
        }

        // Update UI (Selected list)
        if (SelectedListPanel.Instance != null)
        {
            SelectedListPanel.Instance.RefreshForBase(targetBase);
        }
    }



}
