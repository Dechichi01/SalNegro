﻿using UnityEngine;
using System.Collections;

public class MoveCharacterInAnim : StateMachineBehaviour {

    //Assigned in the inspector
    public AnimationCurve movementCurveX;
    public AnimationCurve movementCurveY;
    public float multiplierX = 1f, multiplierY = 1f;

    float prevX, prevY;
    float x, y;

    Controller2D controller;
    int sign;
	 // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        controller = animator.GetComponent<Controller2D>();
        sign = controller.GetComponent<LivingEntity>().states.facingRight ?1:-1;
        prevX = prevY = 0;       
	}

	// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
	override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        x = movementCurveX.Evaluate(stateInfo.normalizedTime)*sign*multiplierX;
        y = movementCurveY.Evaluate(stateInfo.normalizedTime)*multiplierY;

        controller.Move(new Vector3(x - prevX, y-prevY));

        prevX = x;
        prevY = y;
	}

	// OnStateExit is called when a transition ends and the state machine finishes evaluating this state
	//override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	//
	//}

	// OnStateMove is called right after Animator.OnAnimatorMove(). Code that processes and affects root motion should be implemented here
	//override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	//
	//}

	// OnStateIK is called right after Animator.OnAnimatorIK(). Code that sets up animation IK (inverse kinematics) should be implemented here.
	//override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	//
	//}
}
