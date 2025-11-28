using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Weapon : MonoBehaviour
{
    public WeaponEffect weaponEffect;
    private Base ownerBase;
    UnitStats unitStats;

    void Awake()
    {
    }

    void Start()
    {
        ownerBase = GetComponentInParent<Unit>().Base;
        unitStats = GetComponentInParent<UnitStats>();
        GameObject parent = transform.parent.gameObject;
        string parentLayerName = LayerMask.LayerToName(parent.layer);
        int targetLayer = LayerMask.NameToLayer(parentLayerName + "W");

        if (unitStats.unitType == UnitType.Worker)
        {
            targetLayer = LayerMask.NameToLayer("Shovel");
        }

        gameObject.layer = targetLayer;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (unitStats.unitType == UnitType.Worker)
        {
            var square = other.GetComponent<Square>();
            if (square == null) return;
            if (ownerBase != null && square.ownerBase && square.ownerBase.teamID == ownerBase.teamID) return;
            square.TakeDamage(unitStats.damage, ownerBase);
            return;
        }
        else
        {
            var target = other.GetComponent<Unit>();
            if (target == null || target.Base == null) return;
            if (ownerBase != null && target.Base.teamID == ownerBase.teamID) return;
            target.TakeDamage(unitStats.damage, ownerBase);
        }
    }
}
