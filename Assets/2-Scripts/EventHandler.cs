using UnityEngine;
using System.Collections;

public class EventHandler : MonoBehaviour {

    Player player;
    CharacterMovementController charMov;

    public LayerMask normalMask, invincibilityMask;

    private void Start()
    {
        player = FindObjectOfType<Player>();
        charMov = player.GetComponent<CharacterMovementController>();
    }

	public void ChangeToNormalState()
    {
        player.canPerformAction = true;
        player.gameObject.layer = LayerMask.NameToLayer("Player");
        charMov.useGravity = true;
    }

    public void ChangeToActionState(bool immune)
    {        
        player.canPerformAction = false;
        player.gameObject.layer = immune ? LayerMask.NameToLayer("PlayerInvincible") : LayerMask.NameToLayer("Player");
        charMov.useGravity = false;
    }
}
