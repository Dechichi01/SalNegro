using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CharacterMovementController))]
public class CharacterAnimController : MonoBehaviour {

    Player player;
    CharacterMovementController charMov;

    EventHandler eventHandler;
    Animator anim;

	// Use this for initialization
	void Start () {
        anim = GetComponentInChildren<Animator>();
        eventHandler = FindObjectOfType<EventHandler>();
        charMov = GetComponent<CharacterMovementController>();
        player = GetComponent<Player>();
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
        anim.SetTrigger("turn");
        player.canPerformAction = false;
    }

    public void Move(Vector2 input)
    {
        anim.SetFloat("horizontal", charMov.Move(input));
    }
    
    public void Jump()
    {
        anim.SetTrigger("jump");
        charMov.Jump();
    }
	
}
