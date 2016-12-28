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
        player = GetComponent<Player>();
        eventHandler = FindObjectOfType<EventHandler>();
    }

    private void Update()
    {
        if (checkGround)
            anim.SetBool("onAir", !(player.CheckGroundAnim() || player.playerStates.grounded));
    }
    public void Move(Vector2 velocity)
    {
        velocity = controller.Move(velocity);
        anim.SetFloat("horizontal", Mathf.Abs(velocity.x) / (player.moveSpeed*Time.deltaTime));
    }

    public void Attack()
    {
        eventHandler.ChangeToActionState(false);
        anim.SetTrigger("attack");
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
