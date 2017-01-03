using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerController2D : Character2D {

    MobileInputManager inputManager;

    protected override void Start()
    {
        base.Start();
        inputManager = GetComponent<MobileInputManager>();
    }

    protected override void Update()
    {
        //PCTest
        if (states.canPerformAction)
        {
            if (Input.GetKey(KeyCode.RightArrow)) actionsQueue.Enqueue(ActionType.MoveRight);
            if (Input.GetKey(KeyCode.LeftArrow)) actionsQueue.Enqueue(ActionType.MoveLeft);
            if (Input.GetKeyDown(KeyCode.Space)) actionsQueue.Enqueue(ActionType.Jump);
            if (Input.GetKeyDown(KeyCode.Z)) actionsQueue.Enqueue(ActionType.Attack);
            if (Input.GetKeyDown(KeyCode.X)) actionsQueue.Enqueue(ActionType.Roll);
        }


        /*for (int i = 0; i < inputManager.touchInputs.Length; i++)
        {
            if (inputManager.touchInputs[i].pressed && states.canPerformAction)
                actionsQueue.Enqueue(inputManager.touchInputs[i].actionType);   
        }*/

        base.Update();
    }

}

