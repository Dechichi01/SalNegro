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

    public AIState aiState = AIState.Idle;
    [Range(0.1f, 5f)]
    public float aiCycleTime = .8f;

    // Wait for every AIBehavior to finish
    private void LateUpdate()
    {
        ApplyActionsAndMovement();
    }
}

public enum AIState
{
    Idle, Chasing, Patrolling, Fighting
}
