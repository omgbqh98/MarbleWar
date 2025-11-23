using System;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-90)]
public class UnitUpgradeService : MonoBehaviour
{
    public static UnitUpgradeService Instance { get; private set; }

    [Header("Units Registry")]
    public List<UnitConfig> allUnits; // kéo thả tất cả SO UnitConfig vào đây

    public event Action<string> OnUnitChanged; // unitId

    UnitsSaveData _save;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this; DontDestroyOnLoad(gameObject);

        _save = UnitsSaveSystem.Load();
        // Khởi tạo mặc định nếu chưa có record
        foreach (var u in allUnits)
        {
            if (!_save.units.ContainsKey(u.unitId))
            {
                var up = new UnitProgress { unlocked = u.defaultUnlocked, levels = new() };
                foreach (var s in u.stats) if (s != null && s.maxLevel > 0) up.levels[s.type] = 0;
                _save.units[u.unitId] = up;
            }
        }
        UnitsSaveSystem.Save(_save);
    }

    public bool IsUnlocked(string unitId) => _save.units.TryGetValue(unitId, out var up) && up.unlocked;

    public int GetLevel(string unitId, StatType type)
        => _save.units.TryGetValue(unitId, out var up) && up.levels.TryGetValue(type, out var lv) ? lv : 0;

    public int GetUpgradeCost(UnitConfig cfg, StatType type)
    {
        var sc = Array.Find(cfg.stats, s => s.type == type);
        if (sc == null) return int.MaxValue;
        int lv = GetLevel(cfg.unitId, type);
        return Mathf.RoundToInt(sc.baseCost * Mathf.Pow(sc.costGrowth, lv));
    }

    // Giá trị hiệu dụng đưa vào gameplay
    public float GetStatValue(UnitConfig cfg, StatType type)
    {
        var sc = Array.Find(cfg.stats, s => s.type == type);
        if (sc == null) return 0f;
        int lv = GetLevel(cfg.unitId, type);
        return sc.baseValue * (1f + sc.perLevelRate * lv);
    }

    public bool TryUnlock(UnitConfig cfg)
    {
        if (IsUnlocked(cfg.unitId)) return true;
        if (!CurrencyManager.Instance.TrySpend(cfg.unlockCost)) return false;
        var up = _save.units[cfg.unitId]; up.unlocked = true;
        UnitsSaveSystem.Save(_save);
        OnUnitChanged?.Invoke(cfg.unitId);
        return true;
    }

    public bool TryUpgrade(UnitConfig cfg, StatType type)
    {
        if (!IsUnlocked(cfg.unitId)) return false;

        var sc = Array.Find(cfg.stats, s => s.type == type);
        if (sc == null) return false;

        int cur = GetLevel(cfg.unitId, type);
        if (cur >= sc.maxLevel) return false;

        // ✨ tiêu POINT thay vì coin
        if (!PointManager.Instance.TrySpendPoint(1)) return false;

        _save.units[cfg.unitId].levels[type] = cur + 1;
        UnitsSaveSystem.Save(_save);
        OnUnitChanged?.Invoke(cfg.unitId);
        return true;
    }

    public bool TryDowngrade(UnitConfig cfg, StatType type)
    {
        if (!IsUnlocked(cfg.unitId)) return false;

        var sc = System.Array.Find(cfg.stats, s => s.type == type);
        if (sc == null) return false;

        int cur = GetLevel(cfg.unitId, type);
        if (cur <= 0) return false;                 // không hạ dưới 0

        // hạ cấp
        _save.units[cfg.unitId].levels[type] = cur - 1;
        UnitsSaveSystem.Save(_save);

        // hoàn lại 1 point
        PointManager.Instance.RefundPoints(1);

        OnUnitChanged?.Invoke(cfg.unitId);
        return true;
    }


    public IEnumerable<UnitConfig> AllUnits() => allUnits;

    // Tổng số point đã phân trên TẤT CẢ unit
    public int GetTotalPointsSpentAll()
    {
        int sum = 0;
        foreach (var kv in _save.units)
            foreach (var lv in kv.Value.levels)
                sum += lv.Value;
        return sum;
    }

    // Reset tất cả unit về Lv0 và hoàn trả toàn bộ point
    public bool TryResetAllUnits()
    {
        int refund = GetTotalPointsSpentAll();
        if (refund <= 0) return false;

        foreach (var kv in _save.units)
        {
            var up = kv.Value;
            var keys = new List<StatType>(up.levels.Keys);
            foreach (var k in keys) up.levels[k] = 0;
        }

        UnitsSaveSystem.Save(_save);
        PointManager.Instance.RefundPoints(refund); // trả point về kho người chơi

        OnUnitChanged?.Invoke(null); // null = thay đổi diện rộng → UI nào lắng nghe thì refresh
        return true;
    }

}
