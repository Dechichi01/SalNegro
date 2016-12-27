using UnityEngine;
using System.Collections;

[RequireComponent (typeof(CharacterController))]
public class CharacterCtrlGravity : MonoBehaviour {

    public float gravity = 9.8f;

    CharacterController charCtrl;

    private void Start()
    {
        charCtrl = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update () {
        charCtrl.SimpleMove(Vector3.down*gravity * Time.deltaTime);
	}


}
