using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(CharacterAnimController))]
public class Player : LivingEntity {

    //Assigned in the inspector
    public Transform groundCheck;
    public LayerMask groundMask;

    //Available for seen in the inspector
    public bool onGround;
    public bool canPerformAction = true;

    CharacterAnimController charAnim;
    CharacterMovementController charMov;
    MobileInputManager inputManager;

    Vector2 movementInput;
    public bool facingRight = true;

    // Use this for initialization
    protected override void Start()
    {
        base.Start();
        inputManager = GetComponent<MobileInputManager>();
        charAnim = GetComponent<CharacterAnimController>();

        //charActions = new CharActions[4];       
    }

    // Update is called once per frame
    void Update () {
        onGround = CheckGround();
        charAnim.Move(movementInput);
        movementInput = Vector3.zero;

        //Process action (2 per frame per frame)
        for (int i = 0; i < inputManager.touchInputs.Length; i++)
        {
            if (inputManager.touchInputs[i].pressed && canPerformAction)
            {
                ProcessTouch(inputManager.touchInputs[i].actionType);
            }
        }
    }

    void ProcessTouch(ActionType actionType)
    {
        switch(actionType)
        {
            case ActionType.Attack:
                charAnim.Attack();
                break;
            case ActionType.Jump:
                charAnim.Jump();
                break;
            case ActionType.Roll:
                charAnim.Roll();
                break;
            case ActionType.MoveRight:
                if (!facingRight) charAnim.Turn();
                facingRight = true;
                movementInput.x = 1;
                break;
            case ActionType.MoveLeft:
                if (facingRight) charAnim.Turn();
                facingRight = false;
                movementInput.x = -1;
                break;
        }
    }

    bool CheckGround()
    {
        return Physics.Raycast(groundCheck.position, Vector3.down, .05f, groundMask);
    }
}
