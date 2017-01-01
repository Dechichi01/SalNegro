using UnityEngine;
using System.Collections;

public class Controller2D : RaycastController
{
    //Assigned in the inspector
    public LayerMask collisionMask;
    public float maxSlopeAngle = 50f;
    //Movement related variables
    public float moveSpeed = 7f;
    public float accTimeGround = .1f;
    public float accTimeAir = .2f;

    public float jumpHeight = 3.5f;
    public float timeToJumpApex = .6f;

    float gravity;
    float jumpVelocity;
    //

    Vector2 velocity;
    float velocityXSmooth;

    public Transform groundCheck;

    public CollisionInfo collisions;

    [HideInInspector]
    public Vector2 moveInput;

    protected override void Start()
    {
        base.Start();
        gravity = -(2 * jumpHeight) / Mathf.Pow(timeToJumpApex, 2);
        jumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
    }

    public Vector2 Move(Vector2 velocity, bool standingOnPlatform = false)
    {
        UpdateRaycastOrigins();
        collisions.Reset(velocity);

        //Check Collisions
        if (velocity.y < 0)
            DescendSlope(ref velocity);
        if (velocity.x != 0)
            HorizontalCollisions(ref velocity);
        if (velocity.y != 0)
            VerticalCollisions(ref velocity, standingOnPlatform);

        if (standingOnPlatform) collisions.below = true;
        //Move player with the new velocity
        transform.Translate(velocity);

        return velocity;
    }

    #region CollisionCheck

    void HorizontalCollisions(ref Vector2 velocity)
    {
        float directionX = Mathf.Sign(velocity.x);
        float rayLength = Mathf.Abs(velocity.x) + skinWidth;

        for (int i = 0; i < horizontalRayCount; i++)
        {
            //Decides where to shoot the ray from
            Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;
            rayOrigin += Vector2.up * (horizontalRaySpacing * i);

            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);

            Debug.DrawRay(rayOrigin, Vector2.right * directionX * rayLength, Color.red);

            if (hit)
            {
                if (hit.distance == 0 || hit.collider.CompareTag("Platform")) continue;//Don't bother if a object passes through

                //Check for slopes
                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
                if (i == 0 && slopeAngle <= maxSlopeAngle)
                {
                    //Are we climbing a new slope and didn't get there yet?
                    if (slopeAngle != collisions.slopeAngle && hit.distance > skinWidth)
                        ClimbSlope(ref velocity, slopeAngle, hit.normal);
                }
                
                //Not climbing slope or hitting and obstacle            
                if (!collisions.climbingSlope || slopeAngle > maxSlopeAngle)
                {
                    velocity.x = (hit.distance - skinWidth) * directionX;//Subtract skinWidth due to adding it on rayLength;
                    rayLength = hit.distance;

                    if (collisions.climbingSlope)//Fix bounces when hitting horizontal obstacle while climbing slope
                        velocity.y = Mathf.Tan(collisions.slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(velocity.x);

                    collisions.left = directionX == -1;
                    collisions.right = directionX == 1;
                }               
            }
        }
    }

    void VerticalCollisions(ref Vector2 velocity, bool standingOnPlatform)
    {
        float directionY = Mathf.Sign(velocity.y);
        float rayLength = Mathf.Abs(velocity.y) + skinWidth;

        for (int i = 0; i < verticalRayCount; i++)
        {
            //Decides where to shoot the ray from
            Vector2 rayOrigin = (directionY == -1) ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;
            rayOrigin += Vector2.right * (verticalRaySpacing * i + velocity.x);

            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, collisionMask);

            Debug.DrawRay(rayOrigin, Vector2.up * directionY * rayLength, Color.red);

            if (hit)
            {
                if (IgnoreCollision(ref velocity, hit, directionY, standingOnPlatform)) continue;

                collisions.below = directionY == -1;
                collisions.above = directionY == 1;
                velocity.y = (hit.distance - skinWidth) * directionY;
                rayLength = hit.distance;

                if (collisions.climbingSlope)
                    velocity.x = Mathf.Sign(velocity.x) * Mathf.Abs(velocity.y) / Mathf.Tan(collisions.slopeAngle * Mathf.Deg2Rad);
            }
        }

        //Prevents us from moving too far in a frame in a 2 slopes case
        if (collisions.climbingSlope)
        {
            float directionX = Mathf.Sign(velocity.x);
            rayLength = Mathf.Abs(velocity.x) + skinWidth;
            //ray origin from our new hight
            Vector2 rayOrigin = ((directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight) + Vector2.up * velocity.y;
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);

            if (hit)
            {
                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
                if (slopeAngle != collisions.slopeAngle)//new slope?
                {
                    velocity.x = (hit.distance - skinWidth) * directionX;
                    collisions.slopeAngle = slopeAngle;
                    collisions.slopeNormal = hit.normal;
                }
            }
        }
    }

    bool IgnoreCollision(ref Vector2 velocity, RaycastHit2D hit, float direction, bool standingOnPlatform)
    {
        if (hit.collider.CompareTag("Platform"))
        {
            if (direction == 1) return true;//Jumping and hitting a platform
            if (hit.distance <= skinWidth && !standingOnPlatform)
            {
                velocity.y = 0;
                return true;
            }
        }
        return false;
    }
    #endregion

    #region Slopes
    void ClimbSlope(ref Vector2 velocity, float slopeAngle, Vector2 slopeNormal)
    {
        if (collisions.descendingSlope)
        {
            collisions.descendingSlope = false;
            velocity = collisions.velocityOld;
        }

        float moveDistance = Mathf.Abs(velocity.x);
        float climbVelocityY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;

        //Are we climbing the slope (we could jump in the slope)
        if (velocity.y <= climbVelocityY)
        {
            velocity.y = climbVelocityY;
            velocity.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(velocity.x);
            collisions.below = true;//since we're climbing a slope
            collisions.climbingSlope = true;
            collisions.slopeAngle = slopeAngle;
            collisions.slopeNormal = slopeNormal;
        }
    }

    void DescendSlope(ref Vector2 velocity)
    {

        RaycastHit2D maxSlopeHitLeft = Physics2D.Raycast(raycastOrigins.bottomLeft, Vector2.down, Mathf.Abs(velocity.y) + skinWidth, collisionMask);
        RaycastHit2D maxSlopeHitRight = Physics2D.Raycast(raycastOrigins.bottomRight, Vector2.down, Mathf.Abs(velocity.y) + skinWidth, collisionMask);
        if (maxSlopeHitLeft ^ maxSlopeHitRight)
        {
            SlideDownMaxSlope(maxSlopeHitLeft, ref velocity);
            SlideDownMaxSlope(maxSlopeHitRight, ref velocity);
        }

        if (collisions.slidingDownMaxSlope) return;

        float directionX = Mathf.Sign(velocity.x);
        Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomRight : raycastOrigins.bottomLeft;
        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, -Vector2.up, Mathf.Infinity, collisionMask);

        if (hit)
        {
            float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
            if (slopeAngle != 0 && slopeAngle <= maxSlopeAngle)
            {
                if (Mathf.Sign(hit.normal.x) == directionX)//Are we in fact descending the slope?
                {
                    float remainingYDist = Mathf.Tan(slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(velocity.x);
                    if (hit.distance - skinWidth <= remainingYDist)//Are we close enough to the slope for it to take effect
                    {
                        float moveDistance = Mathf.Abs(velocity.x);

                        float descendVelocityY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;
                        velocity.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(velocity.x);
                        velocity.y -= descendVelocityY;

                        collisions.slopeAngle = slopeAngle;
                        collisions.descendingSlope = true;
                        collisions.below = true;
                        collisions.slopeNormal = hit.normal;
                    }
                }
            }
        }
    }

    void SlideDownMaxSlope(RaycastHit2D hit, ref Vector2 moveAmount)
    {

        if (hit)
        {
            float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
            if (slopeAngle > maxSlopeAngle)
            {
                moveAmount.x = Mathf.Sign(hit.normal.x) * (Mathf.Abs(moveAmount.y) - hit.distance) / Mathf.Tan(slopeAngle * Mathf.Deg2Rad);

                collisions.slopeAngle = slopeAngle;
                collisions.slidingDownMaxSlope = true;
                collisions.slopeNormal = hit.normal;
            }
        }

    }

    #endregion

    public Vector2 ProcessMovementInput(Vector2 _moveInput, LivingEntityStates states)
    {
        moveInput = _moveInput;

        states.grounded = collisions.below;

        //Not accumulate gravity
        if (collisions.above || collisions.below)
        {
            if (collisions.slidingDownMaxSlope)
                velocity.y += collisions.slopeNormal.y * -gravity * Time.deltaTime;
            else
                velocity.y = 0;
        }

        //Calculate velocity.x
        float targetVX = moveSpeed * moveInput.x;
        velocity.x = Mathf.SmoothDamp(velocity.x, targetVX, ref velocityXSmooth, states.grounded ? accTimeGround : accTimeAir);

        //Calculate velocity.y
        if (moveInput.y > 0)//Jump
        {
            if (collisions.slidingDownMaxSlope)
            {
                if (moveInput.x != -Mathf.Sign(collisions.slopeNormal.x))//not jumping againt max slope
                {
                    velocity.y = jumpVelocity * collisions.slopeNormal.y;
                    velocity.x = jumpVelocity * collisions.slopeNormal.x;
                }
            }
            else
                velocity.y = moveInput.y * jumpVelocity;
        }

        if (states.useGravity)
            velocity.y += gravity * Time.deltaTime;

  
        return velocity*Time.deltaTime;
    }

    public bool CheckGroundAnim()
    {
        return Physics2D.Raycast(groundCheck.position, Vector3.down, 1f, collisionMask);
    }
}

[System.Serializable]
public struct CollisionInfo
{
    public bool above, below;
    public bool left, right;

    public bool climbingSlope;
    public bool descendingSlope;
    public bool slidingDownMaxSlope;

    public float slopeAngle, slopeAngleOld;

    public Vector2 slopeNormal;
    public Vector2 velocityOld;

    public void Reset(Vector3 oldVelocity)
    {
        above = below = false;
        left = right = false;
        climbingSlope = false;
        descendingSlope = false;
        slidingDownMaxSlope = false;
        slopeNormal = Vector2.zero;

        slopeAngleOld = slopeAngle;
        slopeAngle = 0;

        velocityOld = oldVelocity;
    }
}

