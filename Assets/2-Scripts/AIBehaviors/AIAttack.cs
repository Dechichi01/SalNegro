using UnityEngine;
using Utilities;
using System.Collections;

public class AIAttack : AIBase {

    public float attackDistTreshold = 0.8f;
    public FloatInterval attackDelayInterval;

    float nextCheckTime;
    
    Transform target;
    
    float attackDelay;
    ProbabilityElement<int>[] attackTypes;
    public bool aboutToAttack = false;

    protected override void Start()
    {
        base.Start();
        target = FindObjectOfType<PlayerController2D>().transform;
        nextCheckTime = Time.time + aiControl.aiCycleTime;

        attackTypes = new ProbabilityElement<int>[2];
        attackTypes[0] = new ProbabilityElement<int>(1, 0.7f);
        attackTypes[1] = new ProbabilityElement<int>(2, .3f);
    }

    // Update is called once per frame
    void Update () {

        if (aiControl.aiState == AIState.Patrolling) return;

        float dist = target.transform.position.x - transform.position.x;

        if (Time.time > nextCheckTime)
        {
            FacePlayer(dist);
            nextCheckTime = Time.time + aiControl.aiCycleTime;
            if (Mathf.Abs(dist) > attackDistTreshold) aiControl.aiState = AIState.Chasing;
            else aiControl.aiState = AIState.Fighting;

            if (aiControl.aiState == AIState.Fighting && !aboutToAttack) StartCoroutine(AboutToAttack());
        }
	}

    public void FacePlayer(float dist)
    {
        if (aiControl.states.isAttacking) return;
        if ((dist < 0 && aiControl.states.facingRight) || dist > 0 && !aiControl.states.facingRight)
        {
            aiControl.states.facingRight = dist > 0;
            aiControl.Turn();
        }
    }

    IEnumerator AboutToAttack()
    {
        aboutToAttack = true;
        attackDelay = Random.Range(attackDelayInterval.start, attackDelayInterval.end);

        yield return new WaitForSeconds(attackDelay);

        Attack();
        yield return new WaitForEndOfFrame();

        if (Randomness.GetRandomValue(attackTypes) == 2)
            Attack();

        aboutToAttack = false;
    }
    
    void Attack()
    {
        float dist = target.transform.position.x - transform.position.x;
        if (Mathf.Abs(dist) < attackDistTreshold) aiControl.actionsQueue.Enqueue(ActionType.Attack);
        else aiControl.aiState = AIState.Chasing;
    }
}
