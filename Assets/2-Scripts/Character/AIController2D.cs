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

    //Assigned in the inspector
    public const AIState aiInitialState = AIState.Patrolling;
    public Transform AIBehaviorsHolder;

    public AIState aiState;

    AIBase[] aiBehaviors;

    [Range(0.1f, 5f)]
    public float aiCycleTime = .8f;

    protected override void Start()
    {
        base.Start();
        aiBehaviors = AIBehaviorsHolder.GetComponents<AIBase>();
        Debug.Log(aiBehaviors.Length);
        foreach (AIBase baase in aiBehaviors)
            Debug.Log(baase.GetType());
    }

    // Wait for every AIBehavior to finish
    private void Update()
    {
        foreach (AIBase aiBehav in aiBehaviors)
            aiBehav.ProcessAICycle();

        if (aiState != AIState.Patrolling)//Movement done automatically on patrolling
            ApplyActionsAndMovement();
    }

    public void Turn()
    {
        animControl.Turn();
    }


}

public enum AIState
{
    Idle, Chasing, Patrolling, Fighting
}
