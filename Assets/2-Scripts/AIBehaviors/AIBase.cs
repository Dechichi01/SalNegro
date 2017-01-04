using UnityEngine;
using System.Collections;

/*Simple base class for AIBehaviours, just make sure everybody has the AIController2D component*/
public abstract class AIBase : MonoBehaviour {

    protected AIController2D aiControl;
	// Use this for initialization
	protected virtual void Start () {
        aiControl = transform.root.GetComponent<AIController2D>();
	}

    public abstract void ProcessAICycle();
	
}
