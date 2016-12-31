using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Controller2D))]
public class AnimController2D : MonoBehaviour {

    Player player;
    Controller2D controller;
    EventHandler eventHandler;

    Animator anim;

    bool checkGround = true;
    
    private void Start()
    {
        controller = GetComponent<Controller2D>();
        anim = GetComponentInChildren<Animator>();
        player = (Player) GetComponent<LivingEntity>();
        eventHandler = FindObjectOfType<EventHandler>();
    }

    private void Update()
    {
        if (checkGround)
            anim.SetBool("onAir", !(player.CheckGroundAnim() || player.states.grounded));
    }
    public void Move(Vector2 velocity, Vector2 moveInput)
    {
        velocity = controller.Move(velocity, moveInput);

        float moveSpeed = player.moveSpeed;
        if (controller.collisions.climbingSlope || controller.collisions.descendingSlope)
            moveSpeed *= Mathf.Cos(controller.collisions.slopeAngle * Mathf.Deg2Rad);

        anim.SetFloat("horizontal", Mathf.Abs(velocity.x) / (moveSpeed*Time.deltaTime));
    }

    public void Attack()
    {
        eventHandler.ChangeToActionState(false);
        RaycastHit2D hit = Physics2D.Raycast(player.groundCheck.position, Vector2.down, 8f, controller.collisionMask);
        if (hit)
        {
            if (hit.distance > 0.8 * player.jumpHeight) anim.SetTrigger("airAttack");
            else if (hit.distance == 0) anim.SetTrigger("attack");
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
