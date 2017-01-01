﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(AnimController2D))]
public class Player : LivingEntity {

    MobileInputManager inputManager;
    AnimController2D animControl;
    Controller2D controller;

    //Movement related variables
    Vector2 moveInput;

    protected override void Start()
    {
        base.Start();
        animControl = GetComponent<AnimController2D>();
        controller = GetComponent<Controller2D>();
        inputManager = GetComponent<MobileInputManager>();
    }

    private void Update()
    {
        //PCTest
        if (Input.GetKey(KeyCode.RightArrow)) ProcessTouch(ActionType.MoveRight);
        if (Input.GetKey(KeyCode.LeftArrow)) ProcessTouch(ActionType.MoveLeft);
        if (Input.GetKeyDown(KeyCode.Space)) ProcessTouch(ActionType.Jump);
        if (Input.GetKeyDown(KeyCode.Z)) ProcessTouch(ActionType.Attack);
        if (Input.GetKeyDown(KeyCode.X)) ProcessTouch(ActionType.Roll);

        /*
        for (int i = 0; i < inputManager.touchInputs.Length; i++)
        {
            if (inputManager.touchInputs[i].pressed && states.canPerformAction)
                ProcessTouch(inputManager.touchInputs[i].actionType);
        }*/

        animControl.Move(controller.ProcessMovementInput(moveInput, states));
        moveInput = Vector2.zero;

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

