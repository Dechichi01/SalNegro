using UnityEngine;
using System.Collections;

[RequireComponent(typeof(FieldOfView2D))]
public class AIVision : AIBase {

    public AIState stateWhenPlayerInSight;
    public bool chaseIfLostSight;

    AIState stateBeforeSight;
    bool targetSightLastFrame = false;

    FieldOfView2D fov;

    protected override void Start()
    {
        base.Start();
        fov = GetComponent<FieldOfView2D>();
    }

    private void Update()
    {
        if(fov.visibleTargets.Count > 0)
        {
            if (!targetSightLastFrame)
            {
                stateBeforeSight = aiControl.aiState;
                aiControl.aiState = stateWhenPlayerInSight;
                targetSightLastFrame = true;
            }
        }
        else if (targetSightLastFrame)
        {
            targetSightLastFrame = false;
            aiControl.aiState = chaseIfLostSight ? AIState.Chasing : stateBeforeSight;
        }
    }
}
