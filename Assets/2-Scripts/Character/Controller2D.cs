using UnityEngine;
using System.Collections;

public class Controller2D : RaycastController
{
    //Assigned in the inspector
    public LayerMask collisionMask;

    public float maxClimbAngle = 35f;
    public float maxDescendAngle = 35f;

    public CollisionInfo collisions;

    public Vector2 Move(Vector3 velocity, bool standingOnPlatform = false)
    {
        UpdateRaycastOrigins();
        collisions.Reset(velocity);
        //Check Collisions
        if (velocity.y < 0)
            DescendSlope(ref velocity);
        if (velocity.x != 0)
            HorizontalCollisions(ref velocity);
        if (velocity.y != 0)
            VerticalCollisions(ref velocity);

        if (standingOnPlatform) collisions.below = true;
        //Move player with the new velocity
        transform.Translate(velocity);

        return velocity;
    }

    #region CollisionCheck

    void HorizontalCollisions(ref Vector3 velocity)
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
                if (hit.distance == 0) continue;//Don't bother if a object passes through

                //Check for slopes
                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
                if (i == 0 && slopeAngle <= maxClimbAngle)
                {
                    //Are we climbing a new slope and didn't get there yet?
                    if (slopeAngle != collisions.slopeAngle && hit.distance > skinWidth)
                        ClimbSlope(ref velocity, slopeAngle);
                }
                
                //Not climbing slope or hitting and obstacle            
                if (!collisions.climbingSlope || slopeAngle > maxClimbAngle)
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

    void VerticalCollisions(ref Vector3 velocity)
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
                velocity.y = (hit.distance - skinWidth) * directionY;
                rayLength = hit.distance;

                if (collisions.climbingSlope)
                    velocity.x = Mathf.Sign(velocity.x) * Mathf.Abs(velocity.y) / Mathf.Tan(collisions.slopeAngle * Mathf.Deg2Rad);

                collisions.below = directionY == -1;
                collisions.above = directionY == 1;
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
                }
            }
        }
    }
    #endregion

    #region Slopes
    void ClimbSlope(ref Vector3 velocity, float slopeAngle)
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
        }
    }

    void DescendSlope(ref Vector3 velocity)
    {
        float directionX = Mathf.Sign(velocity.x);
        Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomRight : raycastOrigins.bottomLeft;
        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, -Vector2.up, Mathf.Infinity, collisionMask);

        if (hit)
        {
            float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
            if (slopeAngle != 0 && slopeAngle <= maxDescendAngle)
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
                    }
                }
            }
        }
    }

    #endregion

}

[System.Serializable]
public struct CollisionInfo
{
    public bool above, below;
    public bool left, right;

    public bool climbingSlope;
    public bool descendingSlope;
    public float slopeAngle, slopeAngleOld;

    public Vector3 velocityOld;

    public void Reset(Vector3 oldVelocity)
    {
        above = below = false;
        left = right = false;
        climbingSlope = false;
        descendingSlope = false;

        slopeAngleOld = slopeAngle;
        slopeAngle = 0;

        velocityOld = oldVelocity;
    }
}

