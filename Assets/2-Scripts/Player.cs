using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(AnimController2D))]
public class Player : LivingEntity {

    //Assigned in the inspector
    public float moveSpeed = 7f;
    public float accTimeGround = .1f;
    public float accTimeAir = .2f;

    public float jumpHeight = 3.5f;
    public float timeToJumpApex = .6f;

    public Transform groundCheck;

    float gravity;
    float jumpVelocity;

    const float skinWidth = .015f;


    MobileInputManager inputManager;
    AnimController2D animControl;
    Controller2D controller;

    //Movement related variables
    Vector2 moveInput, velocity;

    float velocityXSmooth;

    protected override void Start()
    {
        base.Start();
        animControl = GetComponent<AnimController2D>();
        controller = GetComponent<Controller2D>();
        inputManager = GetComponent<MobileInputManager>();

        gravity = -(2 * jumpHeight) / Mathf.Pow(timeToJumpApex, 2);
        jumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
    }

    private void Update()
    {
        for (int i = 0; i < inputManager.touchInputs.Length; i++)
        {
            if (inputManager.touchInputs[i].pressed && states.canPerformAction)
                ProcessTouch(inputManager.touchInputs[i].actionType);
        }

        ProcessMovementInput();
    }

    void ProcessTouch(ActionType action)
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

    void ProcessMovementInput()
    {
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
        velocity.x = Mathf.SmoothDamp(velocity.x, targetVX, ref velocityXSmooth, states.grounded ?accTimeGround:accTimeAir);

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

        //Check hard fall
        if (velocity.y < -11f) animControl.SetBoolOnMechanim("hardFall", true);

        animControl.Move(velocity * Time.deltaTime, moveInput);
        //Reset input
        moveInput = Vector2.zero;

    }

    public bool CheckGroundAnim()
    {
        return Physics2D.Raycast(groundCheck.position, Vector3.down, 1f, controller.collisionMask);
    }
}

