using UnityEngine;
using System.Collections;

public class EventHandler : MonoBehaviour {

    Player player;
    Controller2D controller;

    LayerMask maskInNormalState;
    LayerMask maskInImmuneState;

    private void Start()
    {
        player = FindObjectOfType<Player>();
        controller = player.GetComponent<Controller2D>();
        maskInImmuneState = (1 << LayerMask.NameToLayer("Ground")) | (1 << LayerMask.NameToLayer("Wall"));
        maskInNormalState = controller.collisionMask;
    }

	public void ChangeToNormalState()
    {
        player.playerStates.canPerformAction = true;
        player.playerStates.useGravity = true;
        controller.collisionMask = maskInNormalState;
    }

    public void ChangeToActionState(bool immune, bool useGravity)
    {        
        player.playerStates.canPerformAction = false;
        player.playerStates.useGravity = useGravity;
        controller.collisionMask = immune ? maskInImmuneState : maskInNormalState;
    }
}
