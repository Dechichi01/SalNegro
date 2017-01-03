using UnityEngine;
using System.Collections.Generic;
using System.Collections;

/*
 * AICharacter controller.
 * Equivalent to the PlayerController, with the exception that it's process inputs by several AI Behaviors 
 * It's up to the AIBehaviours to assign the actionQueue and/or movement input (see Character2D for examples)
*/
public class AIController2D : Character2D {

    //Assigned in the inspectos
    public bool followTarget;
    public float distanceThreshold = .5f;
    LivingEntity target;

    protected override void Start()
    {
        base.Start();
        target = FindObjectOfType<PlayerController2D>();
    }

    // Update is called once per frame
    protected override void Update () {
        if (followTarget)
        {
            float distFromTarget = transform.position.x - target.transform.position.x;
            float sign = Mathf.Sign(distFromTarget);
            if (controller.collisions.below && Mathf.Abs(distFromTarget) > distanceThreshold)
                moveInput.x = -1 * sign;

            if (states.facingRight != (moveInput.x == 1))
            {
                states.facingRight = moveInput.x == 1;
                animControl.Turn();
            }

        }

        animControl.Move(controller.ProcessMovementInput(moveInput, states), states.facingRight);
        moveInput = Vector2.zero;
    }
}
