using UnityEngine;
using System.Collections;
using System;

[RequireComponent(typeof(FieldOfView2D))]
public class AIVision : AIBase {

    FieldOfView2D fov;

    bool changingState = false;
    protected override void Start()
    {
        base.Start();
        fov = GetComponent<FieldOfView2D>();
    }

    public override void ProcessAICycle()
    {
        if (fov.visibleTargets.Count > 0) aiControl.aiState = AIState.Chasing;
        else if (!changingState) StartCoroutine(ChangeStateToInitial());
    }

    IEnumerator ChangeStateToInitial()
    {
        changingState = true;
        yield return new WaitForSeconds(10f);
        aiControl.aiState = AIController2D.aiInitialState;
        changingState = false;
    }

}
