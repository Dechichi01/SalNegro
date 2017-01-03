using UnityEngine;
using System.Collections;

/*
 * Basic Patrol class, will patrol cycling/non-cycling throught the way points.
 * Works for flying and non flying creatures
*/
public class AIPatrol : AIBase {

    //Assigned in the inspector
    public Vector2[] localPatrolPoints;
    public float speed;
    public bool cyclic;
    public FloatInterval waitTime;
    [Range(0,2)]
    public float easeAmount = 1;
    public bool flying = false;

    Vector2[] patrolPoints;
    int fromPatrolPointIndex, toPatrolPointIndex;
    float percentBetweenPatrolPoints = 0f;
    float nextMoveTime;

    protected override void Start()
    {
        base.Start();

        patrolPoints = new Vector2[localPatrolPoints.Length];
        for (int i = 0; i < localPatrolPoints.Length; i++)
            patrolPoints[i] = localPatrolPoints[i] + (Vector2) transform.position;

        fromPatrolPointIndex = 0;
        toPatrolPointIndex = (fromPatrolPointIndex + 1) % patrolPoints.Length;//+1
    }

    private void Update()
    {
        if (aiControl.aiState == AIState.Patrolling)
            aiControl.Move(CalculateMovement());
    }

    float Ease(float x)
    {
        float a = easeAmount + 1;
        return Mathf.Pow(x, a) / (Mathf.Pow(x, a) + Mathf.Pow(1 - x, a));
    }

    private Vector2 CalculateMovement()
    {
        if (Time.time < nextMoveTime) return Vector2.zero;//Waiting on patrolPoint
        
        if (percentBetweenPatrolPoints < 1)
        {
            float distanceBetweenWaypoints = Vector3.Distance(patrolPoints[fromPatrolPointIndex], patrolPoints[toPatrolPointIndex]);
            percentBetweenPatrolPoints += Time.deltaTime * speed/distanceBetweenWaypoints;
            percentBetweenPatrolPoints = Mathf.Clamp01(percentBetweenPatrolPoints);
            Vector2 newPos = Vector2.Lerp(patrolPoints[fromPatrolPointIndex], patrolPoints[toPatrolPointIndex], Ease(percentBetweenPatrolPoints));
            if (!flying) newPos.y = 0;

            return newPos - (Vector2)transform.position;
        }
        else//Reset
        {
            percentBetweenPatrolPoints = 0f;
            fromPatrolPointIndex = (fromPatrolPointIndex + 1) % patrolPoints.Length;
            toPatrolPointIndex = (fromPatrolPointIndex + 1) % patrolPoints.Length;//+1

            nextMoveTime = Time.time + Random.Range(waitTime.start, waitTime.end);

            return Vector2.zero;
        }


    }

    void OnDrawGizmos()
    {
        if (localPatrolPoints != null)
        {
            Gizmos.color = Color.red;
            float size = .3f;

            for (int i = 0; i < localPatrolPoints.Length; i++)
            {
                Vector3 globalWaypointPos = (Application.isPlaying) ? patrolPoints[i] : localPatrolPoints[i] + (Vector2) transform.position;
                Gizmos.DrawLine(globalWaypointPos - Vector3.up * size, globalWaypointPos + Vector3.up * size);
                Gizmos.DrawLine(globalWaypointPos - Vector3.left * size, globalWaypointPos + Vector3.left * size);
            }
        }
    }
}

[System.Serializable]
public struct FloatInterval
{
    public float start, end;
    public FloatInterval(float _start, float _end) { start = _start; end = _end; }
}
