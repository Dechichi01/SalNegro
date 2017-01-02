using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Controller2D))]
public class AnimController2D : MonoBehaviour {

    Controller2D controller;
    EventHandler eventHandler;

    Animator anim;

    public bool checkGround = true;
    
    private void Start()
    {
        controller = GetComponent<Controller2D>();
        anim = GetComponent<Animator>();
        eventHandler = FindObjectOfType<EventHandler>();
    }

    private void Update()
    {
        if (checkGround)
            anim.SetBool("onAir", !(controller.CheckGroundAnim() || controller.collisions.below));
    }
    public void Move(Vector2 velocity)
    {
        //Check hard fall
        if (velocity.y < -11f) anim.SetBool("hardFall", true);

        velocity = controller.Move(velocity);

        //Calculate horizontal movement percentage
        float moveSpeed = controller.moveSpeed;
        if (controller.collisions.climbingSlope || controller.collisions.descendingSlope)
            moveSpeed *= Mathf.Cos(controller.collisions.slopeAngle * Mathf.Deg2Rad);

        anim.SetFloat("horizontal", velocity.x / (moveSpeed*Time.deltaTime));
    }

    public void Attack()
    {
        eventHandler.ChangeToActionState(false);
        RaycastHit2D hit = Physics2D.Raycast(controller.groundCheck.position, Vector2.down, 8f, controller.collisionMask);
        if (hit)
        {
            if (hit.distance > 0.8 * controller.jumpHeight) anim.SetTrigger("airAttack");
            else anim.SetTrigger("attack");
        }
    }

    public void Roll()
    {
        eventHandler.ChangeToActionState(true);
        anim.SetTrigger("roll");
    }

    public void Turn()
    {
        //eventHandler.ChangeToActionState(false);
        //anim.SetTrigger("turn");
        Vector3 rot = transform.GetChild(0).rotation.eulerAngles;
        transform.GetChild(0).rotation = Quaternion.Euler(rot.x, -rot.y, rot.z);
    }

    public void ClimbLadder()
    {
        anim.SetTrigger("climb");
    }

    public void Jump(float timeToJumpApex)
    {
        anim.SetTrigger("jump");
        SetAirState(timeToJumpApex);
    }

    public void SetAirState(float time)
    {
        anim.SetBool("onAir", true);
        checkGround = false;
        StartCoroutine(EnableCheckGround(time));
    }

    public void SetBoolOnMechanim(string paramName, bool boolean)
    {
        anim.SetBool(paramName, boolean);
    }

    IEnumerator EnableCheckGround(float time)
    {
        yield return new WaitForSeconds(time);
        checkGround = true;
    }
}
