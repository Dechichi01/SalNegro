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

    [HideInInspector]
    private AIState aiInitialState = AIState.Patrolling;

    public AIState aiState;
    [Range(0.1f, 5f)]
    public float aiCycleTime = .8f;

    public bool targetInSight = false;
    bool changedStateBack = false;

    // Wait for every AIBehavior to finish
    private void LateUpdate()
    {
        if (!targetInSight && !changedStateBack)
        {
            changedStateBack = true;
            StartCoroutine(ChangeToOriginalState());
        }
        else if (targetInSight && aiState == AIState.Patrolling) aiState = AIState.Chasing;

        ApplyActionsAndMovement();
    }

    public void Turn()
    {
        animControl.Turn();
    }

    IEnumerator ChangeToOriginalState()
    {
        yield return new WaitForSeconds(25);
        changedStateBack = false;
        if (!targetInSight) aiState = aiInitialState;
    }
}

public enum AIState
{
    Idle, Chasing, Patrolling, Fighting
}
