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

    protected AnimController2D animControl;
    protected Controller2D controller;

    //Input variables (must be assigned by child classes)
    public  Vector2 moveInput;//Can be controlled by the actionsQueue (see ProcessAction function)
    public Queue<ActionType> actionsQueue = new Queue<ActionType>();
    

    protected override void Start ()
    {
        base.Start();
        animControl = GetComponent<AnimController2D>();
        controller = GetComponent<Controller2D>();
    }

    protected void ApplyActionsAndMovement()
    {
        //Movement
        animControl.Move(controller.ProcessMovementInput(moveInput, states), states.facingRight);
        moveInput = Vector2.zero;

        //Actions
        while (actionsQueue.Count > 0)
            ProcessAction(actionsQueue.Dequeue());
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
                    animControl.Jump(controller.timeToJumpApex);
                    moveInput.y = 1;
                }
                break;
            case ActionType.MoveRight:
                if (!states.canMove) break;

                if (!states.facingRight) animControl.Turn();
                states.facingRight = true;
                moveInput.x = 1;
                break;
            case ActionType.MoveLeft:
                if (!states.canMove) break;

                if (states.facingRight) animControl.Turn();
                states.facingRight = false;
                moveInput.x = -1;
                break;
        }
    }
}
