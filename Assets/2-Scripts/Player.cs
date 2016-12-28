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

    public bool canPerformAction = true;
    public bool facingRight = true;
    public bool useGravity = true;
    public bool grounded;
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
            if (inputManager.touchInputs[i].pressed && canPerformAction)
                ProcessTouch(inputManager.touchInputs[i].actionType);
        }

        ProcessMovementInput();

        moveInput = Vector2.zero; 
    }

    void ProcessTouch(ActionType action)
    {
        switch (action)
        {
            case ActionType.Attack:
                animControl.Attack();
                break;  
            case ActionType.Roll:
                animControl.Roll();
                break;
            case ActionType.Jump:
                if (grounded)
                {
                    animControl.SetAirState(timeToJumpApex);
                    moveInput.y = 1;
                }
                break;
            case ActionType.MoveRight:
                if (!facingRight) animControl.Turn();
                facingRight = true;
                moveInput.x = 1;
                break;
            case ActionType.MoveLeft:
                if (facingRight) animControl.Turn();
                facingRight = false;
                moveInput.x = -1;
                break;
        }
    }

    void ProcessMovementInput()
    {
        //Not accumulate gravity
        if (controller.collisions.above || controller.collisions.below)
            velocity.y = 0;

        grounded = controller.collisions.below;

        float targetVX = moveSpeed * moveInput.x;
        velocity.x = Mathf.SmoothDamp(velocity.x, targetVX, ref velocityXSmooth, grounded?accTimeGround:accTimeAir);
        if (moveInput.y > 0)//Jump
            velocity.y = moveInput.y * jumpVelocity;

        if (useGravity)
            velocity.y += gravity * Time.deltaTime;

        if (velocity.y < -5f) animControl.SetBoolOnMechanim("hardFall", true);

        animControl.Move(velocity*Time.deltaTime);
    }

    public bool CheckGround()
    {
        return Physics2D.Raycast(groundCheck.position, Vector3.down, 1.8f, controller.collisionMask);
    }
}
