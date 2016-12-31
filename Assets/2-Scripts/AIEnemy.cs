using UnityEngine;
using System.Collections;

/*Basic Enemy Class that has:
 *  . walking capabilities
 *  . awareness of the player, can follow him and keep a treshold distance
 */

[RequireComponent(typeof(AnimController2D))]
public class AIEnemy : LivingEntity {

    AnimController2D animControl;
    Controller2D controller;

    //Movement related variables
    Vector2 moveInput;

    // Use this for initialization
    protected override void Start()
    {
        base.Start();
        animControl = GetComponent<AnimController2D>();
        controller = GetComponent<Controller2D>();
    }

    // Update is called once per frame
    void Update () {
        animControl.Move(controller.ProcessMovementInput(Vector2.zero, states));
	}
}
