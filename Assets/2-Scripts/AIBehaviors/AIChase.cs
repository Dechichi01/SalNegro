using UnityEngine;
using System.Collections;

public class AIChase : AIBase {

    public AIState stateOnCatch;
    public float distThreshold = 0.5f;

    Transform target;

    protected override void Start()
    {
        base.Start();
        target = FindObjectOfType<PlayerController2D>().transform;
    }

    IEnumerator ChaseCycle()
    {
        if (aiControl.aiState != AIState.Chasing) yield return new WaitForSeconds(aiControl.aiCycleTime);

        /*float distFromTarget = transform.position.x - target.transform.position.x;
        float sign = Mathf.Sign(distFromTarget);
        if (controller.collisions.below && Mathf.Abs(distFromTarget) > distThreshold)
            aiControl.moveInput.x = -1 * sign;

        if (states.facingRight != (moveInput.x == 1))
        {
            states.facingRight = moveInput.x == 1;
            animControl.Turn();
        }*/

    }

}
