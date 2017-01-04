using UnityEngine;
using System.Collections;

public class ChangeParameterOnEnter : StateMachineBehaviour {

    public AnimParameter[] animParameters;

	 // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        foreach (AnimParameter animParameter in animParameters)
            ChangeParameter(animParameter, animator);
	}

    void ChangeParameter(AnimParameter animParameter, Animator anim)
    {
        switch(animParameter.type)
        {
            case ParameterType.BOOL:
                bool value = animParameter.value == "true" ? true : false;
                anim.SetBool(animParameter.name, value);
                break;
            case ParameterType.FLOAT:
                float fNum = float.Parse(animParameter.value);
                anim.SetFloat(animParameter.name, fNum);
                break;
            case ParameterType.INT:
                int tNum = int.Parse(animParameter.value);
                anim.SetInteger(animParameter.name, tNum);
                break;
        }
    }

	// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
	//override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	//
	//}

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

public enum ParameterType { INT, FLOAT, BOOL };

[System.Serializable]
public struct AnimParameter
{
    public string name;
    public string value;
    public ParameterType type;
}
