using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlatformController : RaycastController
{
    public LayerMask passengerMask;
    public Vector3 move;

    List<PassengerMovement> passengerMovement;
    Dictionary<Transform, Controller2D> passengerDictionary = new Dictionary<Transform, Controller2D>();

    protected override void Start()
    {
        base.Start();
        passengerMovement = new List<PassengerMovement>();
    }
    void Update()
    {
        UpdateRaycastOrigins();

        Vector3 velocity = move * Time.deltaTime;

        CalculatePassengerMovement(velocity);
        MovePassengers(true);
        transform.Translate(velocity);
        MovePassengers(false);

    }

    void MovePassengers(bool beforeMovePlatform)
    {
        foreach (PassengerMovement passenger in passengerMovement)
        {
            if (!passengerDictionary.ContainsKey(passenger.transform))
                passengerDictionary.Add(passenger.transform, passenger.transform.GetComponent<Controller2D>());

            if (passenger.moveBeforePlatform == beforeMovePlatform)
                passengerDictionary[passenger.transform].Move(passenger.velocity, passenger.standingOnPlatform);
        }
    }

    void CalculatePassengerMovement(Vector3 velocity)
    {
        HashSet<Transform> movedPassengers = new HashSet<Transform>();
        passengerMovement.Clear();

        float directionX = Mathf.Sign(velocity.x);
        float directionY = Mathf.Sign(velocity.y);

        float rayLength = Mathf.Clamp((Mathf.Abs(velocity.y) + skinWidth), 2 * skinWidth, float.MaxValue);

        //Check for players above platform and move them
        for (int i = 0; i < verticalRayCount; i++)
        {
            Vector2 rayOrigin = raycastOrigins.topLeft + Vector2.right * (verticalRaySpacing * i);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up, rayLength, passengerMask);

            if (hit)
            {
                if (!movedPassengers.Contains(hit.transform))
                {
                    movedPassengers.Add(hit.transform);
                    float pushX, pushY;
                    bool standingOnPlatform = hit.distance <= 2 * skinWidth;
                    Debug.Log(standingOnPlatform);
                    pushX = standingOnPlatform ? velocity.x : 0;
                    pushY = standingOnPlatform ? velocity.y : (velocity.y - (hit.distance - skinWidth) * directionY);

                    passengerMovement.Add(new PassengerMovement(hit.transform, new Vector3(pushX, pushY), standingOnPlatform, directionY == 1));
                }
            }
        }

        //Check for players on the sides of platform and push them
        if (velocity.x != 0)
        {
            rayLength = Mathf.Abs(velocity.x) + skinWidth;

            for (int i = 0; i < horizontalRayCount; i++)
            {
                Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;
                rayOrigin += Vector2.up * (horizontalRaySpacing * i);
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, passengerMask);

                if (hit)
                {
                    if (!movedPassengers.Contains(hit.transform))
                    {
                        movedPassengers.Add(hit.transform);
                        float pushX = velocity.x - (hit.distance - skinWidth) * directionX;//pushPlayer
                        float pushY = -skinWidth; //just so the player know he's on the grouhnd
                        passengerMovement.Add(new PassengerMovement(hit.transform, new Vector3(pushX, pushY), false, true));
                    }
                }
            }
        }
    }

    struct PassengerMovement
    {
        public Transform transform;
        public Vector3 velocity;
        public bool standingOnPlatform;
        public bool moveBeforePlatform;

        public PassengerMovement(Transform _transform, Vector3 _velocity, bool _standingOnPlatform, bool _moveBeforePlatform)
        {
            transform = _transform;
            velocity = _velocity;
            standingOnPlatform = _standingOnPlatform;
            moveBeforePlatform = _moveBeforePlatform;
        }
    }
}
