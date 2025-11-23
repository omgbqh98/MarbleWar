// Stat có thể mở rộng tuỳ unit (tối đa 3 stat/UnitPanel)
using UnityEngine;

public enum StatType { Attack, Health, Speed, CritChance, Armor, Cooldown /*...*/ }

[System.Serializable]
public class StatUpgradeConfig
{
    public StatType type;             // Loại chỉ số
    public float baseValue = 1f;      // Giá trị ban đầu tại Lv0
    public float perLevelRate = 0.15f;// %/hệ số tăng mỗi cấp (vd 0.2 = +20%/lvl)
    public int baseCost = 100;        // giá ở cấp 0
    public float costGrowth = 1.25f;  // hệ số tăng giá mỗi cấp
    public int maxLevel = 50;         // trần cấp
}

[CreateAssetMenu(menuName = "Configs/Unit")]
public class UnitConfig : ScriptableObject
{
    [Header("Identity")]
    public string unitId;             // duy nhất (vd "archer_01")
    public string displayName;
    public Sprite icon;

    [Header("Unlocking")]
    public bool defaultUnlocked = false;
    public int unlockCost = 500;      // cost mở khoá (0 nếu free)

    [Header("Upgrades (max 3 items)")]
    public StatUpgradeConfig[] stats = new StatUpgradeConfig[3]; // 1..3
}
