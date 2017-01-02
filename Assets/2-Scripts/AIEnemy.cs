using UnityEngine;
using System.Collections;

/*Basic Enemy Class that has:
 *  . walking capabilities
 *  . awareness of the player, can follow him and keep a treshold distance
 */

[RequireComponent(typeof(AnimController2D))]
public class AIEnemy : LivingEntity {

    //Assigned in the inspectos
    public bool followTarget;
    public float distanceThreshold = .5f;
    LivingEntity target;
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
        target = FindObjectOfType<Player>();
    }

    // Update is called once per frame
    void Update () {
        float distFromTarget = transform.position.x - target.transform.position.x;
        float sign = Mathf.Sign(distFromTarget);
        if (controller.collisions.below && Mathf.Abs(distFromTarget) > distanceThreshold)
            moveInput.x = -1*sign;

        animControl.Move(controller.ProcessMovementInput(moveInput, states), states.facingRight);
        moveInput = Vector2.zero;
	}
}
