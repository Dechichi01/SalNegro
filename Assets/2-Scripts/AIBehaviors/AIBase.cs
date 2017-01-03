using UnityEngine;
using System.Collections;

/*Simple base class for AIBehaviours, just make sure everybody has the AIController2D component*/
[RequireComponent (typeof(AIController2D))]
public class AIBase : MonoBehaviour {

    protected AIController2D aiControl;
	// Use this for initialization
	protected virtual void Start () {
        aiControl = GetComponent<AIController2D>();
	}
	
}
