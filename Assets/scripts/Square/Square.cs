using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Square : MonoBehaviour
{
    public float currentHP = 100;
    public int expWhenDie = 5;
    public Base ownerBase;
    SpriteRenderer spriteRenderer;
    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void TakeDamage(float dmg, Base attackerBase)
    {
        currentHP -= dmg;
        if (currentHP <= 0) Die(attackerBase);
    }

    void Die(Base attackerBase)
    {
        attackerBase.UpdateExp(expWhenDie);
        ownerBase = attackerBase;

        Color c = attackerBase.baseColor;
        c.a = 0.5f;  // chỉnh alpha (0 = trong suốt, 1 = đậm)
        spriteRenderer.color = c;
    }
}
