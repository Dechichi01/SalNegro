using UnityEngine;
using System.Collections;

//[RequireComponent(typeof(BoxCollider2D))]
public class RaycastController : MonoBehaviour {

    //Assigned in the inspector
    public int horizontalRayCount = 4;
    public int verticalRayCount = 4;

    protected const float skinWidth = .015f;
    protected float horizontalRaySpacing;
    protected float verticalRaySpacing;

    public BoxCollider2D coll;
    protected RaycastOrigins raycastOrigins;

    protected virtual void Awake()
    {
        if (coll == null) coll = GetComponentInChildren<BoxCollider2D>();
    }

    protected virtual void Start()
    {
        CalculateRaySpacing();
    }

    public void UpdateRaycastOrigins()
    {
        Bounds bounds = coll.bounds;
        bounds.Expand(skinWidth * -2);

        raycastOrigins.bottomLeft = new Vector2(bounds.min.x, bounds.min.y);
        raycastOrigins.bottomRight = new Vector2(bounds.max.x, bounds.min.y);
        raycastOrigins.topLeft = new Vector2(bounds.min.x, bounds.max.y);
        raycastOrigins.topRight = new Vector2(bounds.max.x, bounds.max.y);
    }

    public void CalculateRaySpacing()
    {
        Bounds bounds = coll.bounds;
        bounds.Expand(skinWidth * -2);

        horizontalRayCount = Mathf.Clamp(horizontalRayCount, 2, int.MaxValue);
        verticalRayCount = Mathf.Clamp(verticalRayCount, 2, int.MaxValue);

        horizontalRaySpacing = bounds.size.y / (horizontalRayCount - 1);
        verticalRaySpacing = bounds.size.x / (verticalRayCount - 1);
    }
}

public struct RaycastOrigins
{
    public Vector2 topLeft, topRight;
    public Vector2 bottomLeft, bottomRight;
}
