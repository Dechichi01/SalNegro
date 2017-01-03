using UnityEngine;
using System.Collections;

public class PlatformClimbTrigger : MonoBehaviour {

    public Transform startClimbPos;
	// Use this for initialization
	void Start () {
        if (!startClimbPos) startClimbPos = transform.GetChild(0);
	}
}
