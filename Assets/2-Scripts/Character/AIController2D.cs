using UnityEngine;
using System.Collections.Generic;
using System.Collections;

/*
 * AICharacter controller.
 * Equivalent to the PlayerController, with the exception that it's process inputs by several AI Behaviors 
 * -> It's up to the AIBehaviours to assign the actionQueue and/or movement input (see Character2D for examples)
 * -> Can override moveInput and move the character's directly (for more precision movement in Patrols and Follow rotines)
*/
public class AIController2D : Character2D {

    // Wait for every AIBehavior to finish
    private void LateUpdate()
    {
        ApplyActionsAndMovement();
    }

    public void Move(Vector2 deltaPos)
    {
        Debug.Log(deltaPos.x);
        if ((deltaPos.x > 0.01f && !states.facingRight) || (deltaPos.x < -0.01f && states.facingRight))
        {
            states.facingRight = !states.facingRight;
            animControl.Turn();
        }

        animControl.Move(deltaPos, states.facingRight);
    }
}
