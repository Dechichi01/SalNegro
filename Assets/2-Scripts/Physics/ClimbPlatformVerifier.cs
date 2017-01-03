using UnityEngine;
using System.Collections;

public class ClimbPlatformVerifier : RaycastController {

    float rayLength;
    public LayerMask collisionMask;

    LivingEntity livingEntity;
    AnimController2D animCtrl;
    Controller2D controller;

    protected override void Start()
    {
        base.Start();
        rayLength = skinWidth * 2;
        livingEntity = GetComponentInParent<LivingEntity>();
        animCtrl = livingEntity.GetComponent<AnimController2D>();
        controller = livingEntity.GetComponent<Controller2D>();
    }
    // Update is called once per frame
    void Update () {
        UpdateRaycastOrigins();
        float rayDir = livingEntity.states.facingRight ? 1 : -1;
        for (int i = 0; i < horizontalRayCount; i++)
        {
            //Decides where to shoot the ray from
            Vector2 rayOrigin = livingEntity.states.facingRight?raycastOrigins.bottomRight:raycastOrigins.bottomLeft;
            rayOrigin += Vector2.up * (horizontalRaySpacing * i);

            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right*rayDir, rayLength, collisionMask);
            Debug.DrawRay(rayOrigin, Vector2.right * rayDir* rayLength, Color.red);
            if (!livingEntity.states.isClimbing && hit)
            {
                animCtrl.ClimbLadder(hit.collider.GetComponent<PlatformClimbTrigger>().startClimbPos.position);
            }                
        }
    }
}
