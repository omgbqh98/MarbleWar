using UnityEngine;

[System.Serializable]
public class UnitStats : MonoBehaviour
{
    [Header("Loại unit")]
    public UnitType unitType;

    [Header("HP & Phòng ngự")]
    public float maxHP = 100;
    public float currentHP = 100;
    public float armor = 2;

    [Header("Tấn công")]
    public float damage = 10;
    public float critChance = 0.1f;   // 10%
    public float attackSpeed = 1.0f;   // lần/giây
    public float attackSpeedBow = 3.0f;   // lần/giây
    public float attackRange = 1.5f;  // 0-1.5 cận; > tầm bắn

    [Header("Di chuyển")]
    public float moveSpeed = 0.5f;

    public void ApplyModifier(StatModifier mod)
    {
        if (mod == null) return;

        // Flat adds
        maxHP += mod.flatMaxHP;
        currentHP += mod.flatMaxHP;
        armor += mod.flatArmor;
        damage += mod.flatDamage;
        critChance += mod.flatCrit;
        attackSpeed += mod.flatAttackSpeed;
        attackSpeedBow += mod.flatAttackSpeedBow;
        attackRange += mod.flatAttackRange;
        moveSpeed += mod.flatMoveSpeed;

        // Percent multipliers
        maxHP *= (1f + mod.percentMaxHP / 100f);
        currentHP *= (1f + mod.percentMaxHP / 100f);
        armor *= (1f + mod.percentArmor / 100f);
        damage *= (1f + mod.percentDamage / 100f);
        critChance *= (1f + mod.percentCrit / 100f);
        attackSpeed *= (1f + mod.percentAttackSpeed / 100f);
        attackSpeedBow *= (1f + mod.percentAttackSpeedBow / 100f);
        attackRange *= (1f + mod.percentAttackRange / 100f);
        moveSpeed *= (1f + mod.percentMoveSpeed / 100f);
    }
}
