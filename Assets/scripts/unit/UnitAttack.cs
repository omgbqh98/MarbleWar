using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitAttack : MonoBehaviour
{
    [Header("Animation")]
    private Animator animator;
    UnitStats unitStats;
    private Base myBase;
    private float rangeDefault = 1f;
    BowAttact bowAttact;
    private float cooldownLeft = 3f;

    // Start is called before the first frame update
    void Start()
    {
        bowAttact = GetComponent<BowAttact>();
        animator = GetComponent<Animator>();
        unitStats = GetComponent<UnitStats>();
        rangeDefault = unitStats.attackRange;
        if (myBase == null)
            myBase = GetComponent<Unit>().Base;
    }

    // Update is called once per frame
    void Update()
    {
        GameObject target = FindNearestEnemy();
        if (target != null)
            Attack(target);
    }

    public void Attack(GameObject target)
    {
        float distanceToTarget = Vector3.Distance(transform.position, target.transform.position);

        if (distanceToTarget < rangeDefault)
        {
            animator.speed = unitStats.attackSpeed;

            if (bowAttact != null)
            {
                cooldownLeft -= Time.deltaTime;
                if (cooldownLeft <= 0)
                {
                    cooldownLeft = unitStats.attackSpeedBow;
                    animator.SetBool("isAttack", true);
                    StartCoroutine(SetAnimation(false));
                }
            }
            else
            {
                animator.SetBool("isAttack", true);
            }
        }
        else
        {
            animator.SetBool("isAttack", false);
        }
    }

    GameObject FindNearestEnemy()
    {
        Unit[] allUnits = FindObjectsOfType<Unit>();
        GameObject nearest = null;
        float minDist = Mathf.Infinity;

        foreach (Unit other in allUnits)
        {
            // Bỏ qua nếu cùng Base hoặc null
            if (other == null || other.Base == null || other.Base == myBase)
                continue;

            if (other.Base.teamID == myBase.teamID) continue;

            float dist = Vector3.Distance(transform.position, other.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                nearest = other.gameObject;
            }
        }

        return nearest;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, rangeDefault);
    }

    IEnumerator SetAnimation(bool isAttack)
    {
        yield return new WaitForSeconds(0.5f);
        animator.SetBool("isAttack", isAttack);
    }
}
