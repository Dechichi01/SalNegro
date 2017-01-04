using UnityEngine;
using Utilities;
using System.Collections;

public class AIAttack : AIBase {

    public float maxAttackDist = 1.5f;
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

    public override void ProcessAICycle()
    {
        if (aiControl.aiState != AIState.Fighting) return;

        if (Time.time > nextCheckTime)
        {
            nextCheckTime = Time.time + aiControl.aiCycleTime;
            float dist = target.transform.position.x - transform.position.x;
            FacePlayer(dist);
            Debug.Log(dist);
            if (Mathf.Abs(dist) > maxAttackDist) aiControl.aiState = AIState.Chasing;
            else if (!aboutToAttack) StartCoroutine(AboutToAttack());
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
        if (Mathf.Abs(dist) < maxAttackDist) aiControl.actionsQueue.Enqueue(ActionType.Attack);
        else aiControl.aiState = AIState.Chasing;
    }
}
