// StatModifier.cs
using System;

[Serializable]
public class StatModifier
{
    // Flat increments
    public float flatMaxHP = 0;
    public float flatArmor = 0;
    public float flatDamage = 0;
    public float flatCrit = 0;
    public float flatAttackSpeed = 0;
    public float flatAttackSpeedBow = 0;
    public float flatAttackRange = 0;
    public float flatMoveSpeed = 0;

    // ex +10%
    public float percentMaxHP = 0;
    public float percentArmor = 0;
    public float percentDamage = 0;
    public float percentCrit = 0;
    public float percentAttackSpeed = 0;
    public float percentAttackSpeedBow = 0;
    public float percentAttackRange = 0;
    public float percentMoveSpeed = 0;
}
