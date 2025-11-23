using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public Base Base;
    public Sprite spriteVukhi;
    public int maxRow;
    public int maxCol;
    public int expWhenDie = 5;
    UnitStats unitStats;
    [HideInInspector] public GameObject prefabReference;

    void Start()
    {
        unitStats = GetComponent<UnitStats>();
    }

    public void TakeDamage(float dmg, Base attackerBase)
    {
        unitStats.currentHP -= dmg;

        if (unitStats.currentHP <= 0) Die(attackerBase);
    }

    void Die(Base attackerBase)
    {
        attackerBase.UpdateExp(expWhenDie);
        Destroy(gameObject);
    }

    private void OnEnable()
    {
        if (Base != null)
            Base.Register(this);
    }

    private void OnDisable()
    {
        if (Base != null)
            Base.Unregister(this);
    }
}
