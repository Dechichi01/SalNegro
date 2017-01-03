using UnityEngine;
using System.Collections;

[RequireComponent(typeof(FieldOfView2D))]
public class AIVision : AIBase {

    FieldOfView2D fov;

    protected override void Start()
    {
        base.Start();
        fov = GetComponent<FieldOfView2D>();
    }

    private void Update()
    {
        aiControl.targetInSight = fov.visibleTargets.Count > 0;

    }

}
