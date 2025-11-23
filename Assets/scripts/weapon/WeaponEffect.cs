using UnityEngine;
using System.Collections;


public enum EffectType { None, Poison, Fire, Ice, Stun, Knockback }
[CreateAssetMenu(menuName = "Combat/WeaponData")]
public class WeaponEffect : MonoBehaviour
{
    [Header("Damage & bắn")]
    public float damage = 10f;
    public float projectileSpeed = 12f;     // 0 = melee swing (không bay)
    public float lifeTime = 1.5f;
    public int maxPierce = 1;               // số mục tiêu xuyên qua (projectile)

    [Header("Hiệu ứng")]
    public EffectType effect = EffectType.None;

    // Poison/Fire
    public int poisonTicks = 3;
    public float poisonDamagePerTick = 2f;
    public float burnDuration = 2f;
    public float burnDPS = 5f;

    // Ice/Slow
    public float slowDuration = 2f;
    public float slowFactor = 0.5f;

    // Stun
    public float stunDuration = 1f;

    // Knockback
    public float knockbackForce = 5f;

    [Header("VFX")]
    public GameObject hitVFX;
}
