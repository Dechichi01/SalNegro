using UnityEngine;
using System.Collections;

public class ChangeCollisionMaskOnState : StateMachineBehaviour {

    LayerMask maskInNormalState;
    public LayerMask maskOnState;

    public bool reverse = false;
    [Range(0,1)]
    public float reverseTime = 1f;
    bool reversed = false;

    Controller2D controller;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        controller = animator.GetComponent<Controller2D>();
        maskInNormalState = controller.collisionMask;

        controller.collisionMask = maskOnState;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        if (!reversed && reverse && stateInfo.normalizedTime > reverseTime)
        {
            reversed = true;
            controller.collisionMask = maskInNormalState;
        }

    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        controller.collisionMask = maskInNormalState;
        reversed = false;
    }

    // OnStateMove is called right after Animator.OnAnimatorMove(). Code that processes and affects root motion should be implemented here
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
    //
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK(). Code that sets up animation IK (inverse kinematics) should be implemented here.
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
    //
    //}
}
