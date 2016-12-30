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

    public PlayerStates playerStates;
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
        playerStates = new PlayerStates();
    }

    private void Update()
    {
        for (int i = 0; i < inputManager.touchInputs.Length; i++)
        {
            if (inputManager.touchInputs[i].pressed && playerStates.canPerformAction)
                ProcessTouch(inputManager.touchInputs[i].actionType);
        }

        ProcessMovementInput();
    }

    void ProcessTouch(ActionType action)
    {
        switch (action)
        {
            case ActionType.Attack:
                if (playerStates.canAttack)
                    animControl.Attack();
                break;  
            case ActionType.Roll:
                if (playerStates.grounded)
                    animControl.Roll();
                break;
            case ActionType.Jump:
                if (playerStates.grounded)
                {
                    animControl.SetAirState(timeToJumpApex);
                    moveInput.y = 1;
                }
                break;
            case ActionType.MoveRight:
                if (!playerStates.canMove) break;

                if (!playerStates.facingRight) animControl.Turn();
                playerStates.facingRight = true;
                moveInput.x = 1;
                break;
            case ActionType.MoveLeft:
                if (!playerStates.canMove) break;

                if (playerStates.facingRight) animControl.Turn();
                playerStates.facingRight = false;
                moveInput.x = -1;
                break;
        }
    }

    void ProcessMovementInput()
    {
        playerStates.grounded = controller.collisions.below;

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
        velocity.x = Mathf.SmoothDamp(velocity.x, targetVX, ref velocityXSmooth, playerStates.grounded ?accTimeGround:accTimeAir);

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

        velocity.y += gravity * Time.deltaTime;

        //Check hard fall
        if (velocity.y < -11f) animControl.SetBoolOnMechanim("hardFall", true);

        //Reset input
        moveInput = Vector2.zero;

        animControl.Move(velocity*Time.deltaTime);
    }

    public bool CheckGroundAnim()
    {
        return Physics2D.Raycast(groundCheck.position, Vector3.down, 1.8f, controller.collisionMask);
    }
}

[System.Serializable]
public class PlayerStates
{
    public bool canMove = true;
    public bool canAttack = true;
    public bool canPerformAction = true;
    public bool facingRight = true;
    public bool grounded;

    public void Copy(PlayerStates states)
    {
        canMove = states.canMove;
        canAttack = states.canAttack;
        canPerformAction = states.canPerformAction;
    }
}
