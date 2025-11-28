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
        GameObject target = TargetFinder.FindNearestTarget(transform.position, myBase, Mathf.Infinity, 0.05f);
        if (unitStats.unitType == UnitType.Worker)
        {
            target = TargetFinder.FindNearestEnemySquare(transform.position, myBase, Mathf.Infinity);
        }
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
