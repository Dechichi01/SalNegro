using UnityEngine;
using System.Collections;

public class ClimbLadder : StateMachineBehaviour {

    public IntervalValues[] climbFrameIntervals;
    public AnimationCurve movementCurve;

    IntervalValues[] intervalAnimPercents;
    float[] deltaPercentInInterval;//% of the delta to be moved in the frame interval
    int index = 0;

    Vector2 deltaInInterval;
    Vector2 remainingMovement;

    Controller2D controller;
    float sign;
    Vector2 delta;

    Vector2 prevPos;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        //animator.applyRootMotion = true;

        controller = animator.GetComponent<Controller2D>();
        delta = controller.GetComponentInChildren<ClimbPlatformVerifier>().climbLadderStartPos;
        sign = controller.GetComponent<LivingEntity>().states.facingRight ? 1 : -1;

        intervalAnimPercents = new IntervalValues[climbFrameIntervals.Length];
        float movementInterval = 0;
        for (int i = 0; i < intervalAnimPercents.Length; i++)
        {
            float start = climbFrameIntervals[i].start/115f;
            float end = climbFrameIntervals[i].end/115f;
            intervalAnimPercents[i] = new IntervalValues(start, end);
            movementInterval += (end - start);
        }
        deltaPercentInInterval = new float[intervalAnimPercents.Length];
        for (int i = 0; i < deltaPercentInInterval.Length; i++)
        {
            float start = intervalAnimPercents[i].start;
            float end = intervalAnimPercents[i].end;
            deltaPercentInInterval[i] = (end - start) / movementInterval;
        }

        deltaInInterval = deltaPercentInInterval[0] * delta;
        remainingMovement = deltaInInterval;

        prevPos = Vector2.zero;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        //controller.Move(animator.velocity * Time.deltaTime);
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        index = 0;
        //animator.applyRootMotion = false;
    }

    // OnStateMove is called right after Animator.OnAnimatorMove(). Code that processes and affects root motion should be implemented here
    override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        controller.velocity = ((Vector2) animator.velocity * Time.deltaTime);
        controller.Move(controller.velocity);
    }

    // OnStateIK is called right after Animator.OnAnimatorIK(). Code that sets up animation IK (inverse kinematics) should be implemented here.
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
    //
    //}

    [System.Serializable]
    public struct IntervalValues
    {
        public float start, end;
        public IntervalValues(float _start, float _end) { start = _start; end = _end; }
    }
}
