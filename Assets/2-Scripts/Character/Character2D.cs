using UnityEngine;
using System.Collections.Generic;
using System.Collections;

/*
 * Base class for Character (user or AI) in game
 * Utilizes simple raycast collision detection an Mechanim animation control
 * Every Update, utilizes animControl and controller to:
 *      . Perform movement (using Vector2 movement input)
 *      . Perform action based on the user/AI input (using actionsQueue)
 
----> OBS: Any child implementing this class should call ApplyActionsAndMovement() everyframe (even if there isn't any)
for physics to be applied (see PlayerController2D or AIController2D classes for example)<---
 */
[RequireComponent(typeof(AnimController2D))]
public class Character2D : LivingEntity {

    //Movement related variables
    public float moveSpeed = 7f;
    public float accTimeGround = .1f;
    public float accTimeAir = .2f;

    public float jumpHeight = 3.5f;
    public float timeToJumpApex = .6f;

    float gravity;
    float jumpVelocity;
    //

    float velocityXSmooth;

    protected AnimController2D animControl;
    protected Controller2D controller;

    //Input variables (must be assigned by child classes)
    [HideInInspector]
    public Vector2 moveInput;//Can be controlled by the actionsQueue (see ProcessAction function)
    [HideInInspector]
    public Vector2 velocity;

    public Queue<ActionType> actionsQueue = new Queue<ActionType>();
    

    protected override void Start ()
    {
        base.Start();
        animControl = GetComponent<AnimController2D>();
        controller = GetComponent<Controller2D>();

        gravity = -(2 * jumpHeight) / Mathf.Pow(timeToJumpApex, 2);
        jumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
    }

    protected void ApplyActionsAndMovement()
    {
        //Movement
        Move(ProcessMovementInput(moveInput, states));
        moveInput = Vector2.zero;

        //Actions
        while (actionsQueue.Count > 0)
            ProcessAction(actionsQueue.Dequeue());
    }

    public void Move(Vector2 deltaPos, bool calledByAnim = false)
    {
        if (!calledByAnim)
        {
            if ((deltaPos.x > 0.01f && !states.facingRight) || (deltaPos.x < -0.01f && states.facingRight))
            {
                states.facingRight = !states.facingRight;
                animControl.Turn();
            }
        }
        animControl.Move(deltaPos, states.facingRight);
    }

    public override void TakeHit(float damage, Vector3 hitPoint, Vector3 hitDirection)
    {
        //Animation choices based on hit point and hitDirection. Maybe this should be in the animControl2d?
        base.TakeHit(damage, hitPoint, hitDirection);
    }

    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);
    }

    public override void Die()
    {
        animControl.Die();
        base.Die();
    }

    protected virtual void ProcessAction(ActionType action)
    {
        switch (action)
        {
            case ActionType.Attack:
                if (states.canAttack)
                    animControl.Attack();
                break;
            case ActionType.Roll:
                if (states.grounded)
                    animControl.Roll();
                break;
            case ActionType.Jump:
                if (states.grounded)
                {
                    animControl.Jump(timeToJumpApex);
                    moveInput.y = 1;
                }
                break;
            case ActionType.MoveRight:
                if (!states.canMove) break;
                moveInput.x = 1;
                break;
            case ActionType.MoveLeft:
                if (!states.canMove) break;
                moveInput.x = -1;
                break;
        }
    }

    protected Vector2 ProcessMovementInput(Vector2 _moveInput, LivingEntityStates states)
    {
        moveInput = _moveInput;

        states.grounded = controller.collisions.below;

        //Not accumulate gravity
        if (controller.collisions.above || controller.collisions.below)
        {
            if (controller.collisions.slidingDownMaxSlope)
                velocity.y += controller.collisions.slopeNormal.y * -gravity * Time.deltaTime;
            else
                velocity.y = 0;
        }

        //Calculate velocity.x
        float targetVX = moveSpeed * moveInput.x;
        velocity.x = Mathf.SmoothDamp(velocity.x, targetVX, ref velocityXSmooth, states.grounded ? accTimeGround : accTimeAir);

        //Calculate velocity.y
        if (moveInput.y > 0)//Jump
        {
            if (controller.collisions.slidingDownMaxSlope)
            {
                if (moveInput.x != -Mathf.Sign(controller.collisions.slopeNormal.x))//not jumping againt max slope
                {
                    velocity.y = jumpVelocity * controller.collisions.slopeNormal.y;
                    velocity.x = jumpVelocity * controller.collisions.slopeNormal.x;
                }
            }
            else
                velocity.y = moveInput.y * jumpVelocity;
        }

        if (states.useGravity)
            velocity.y += gravity * Time.deltaTime;


        return velocity * Time.deltaTime;
    }

}
