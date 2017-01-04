using UnityEngine;
using System.Collections;
using System;

public class AIChase : AIBase {

    public AIState stateOnCatch;
    public float distThreshold = 1f;
    float nextCheckTime;

    Transform target;

    float moveInputX = 0f;

    protected override void Start()
    {
        base.Start();
        target = FindObjectOfType<PlayerController2D>().transform;
        nextCheckTime = Time.time + aiControl.aiCycleTime;
    }

    public override void ProcessAICycle()
    {
        if (aiControl.aiState != AIState.Chasing) return;

        aiControl.moveInput.x = moveInputX;

        if (Time.time > nextCheckTime)
        {
            nextCheckTime = Time.time + aiControl.aiCycleTime;
            float distFromTarget = transform.position.x - target.transform.position.x;
            float sign = Mathf.Sign(distFromTarget);
            if (aiControl.states.grounded && Mathf.Abs(distFromTarget) > distThreshold)
                moveInputX = -1 * sign;
            else aiControl.aiState = AIState.Fighting;
        }
    }
}
