using UnityEngine;
using System.Collections;

public class CharacterMovementController : MonoBehaviour {

    public float moveSpeed = 12f;
    public float jumpSpeed = 15f;
    public float accTimeGrounded = .1f;
    public float accTimeAir = .2f;
    public float gravity = 20.0F;

    [HideInInspector]
    public bool useGravity = true;

    Vector3 velocity;
    float velocityXSmoothing;

    CharacterController charCtrl;
    Player player;
    // Use this for initialization
	void Start () {
        charCtrl = GetComponent<CharacterController>();
        player = GetComponent<Player>();
	}
	
    //returns a speed % that will be used to set "horizontal" parameter in the animator
    public float Move(Vector2 input)
    {
        float targetVelocityX = input.x * moveSpeed;
        //velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, player.onGround ? accTimeGrounded : accTimeAir);

        charCtrl.Move(velocity * Time.deltaTime);

        return Mathf.Abs(velocity.x / moveSpeed);
    }

}
