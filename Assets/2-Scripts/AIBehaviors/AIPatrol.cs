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

    Vector2[] patrolPoints;
    int fromPatrolPointIndex, toPatrolPointIndex;
    float distanceBetweenPatrolPoints;
    float percentBetweenPatrolPoints = 0f;
    float nextMoveTime;

    bool startingPatrol = false;
    bool patrolStarted = false;

    protected override void Start()
    {
        base.Start();

        patrolPoints = new Vector2[localPatrolPoints.Length];
        for (int i = 0; i < localPatrolPoints.Length; i++)
            patrolPoints[i] = localPatrolPoints[i] + (Vector2) transform.position;

        //Initial calculations
        fromPatrolPointIndex = 0;
        toPatrolPointIndex = (fromPatrolPointIndex + 1) % patrolPoints.Length;//+1
        distanceBetweenPatrolPoints = Vector3.Distance(patrolPoints[fromPatrolPointIndex], patrolPoints[toPatrolPointIndex]);
    }

    public override void ProcessAICycle()
    {
        if (aiControl.aiState == AIState.Patrolling)
        {
            if (patrolStarted) MovePatroller(CalculateMovement());
            else if (!startingPatrol) StartCoroutine(MoveToFirstPatrolPoint());
        }
        else patrolStarted = false ;
    }

    public void MovePatroller(Vector2 moveAmount)
    {        
        Vector2 velocity = aiControl.ApplyPhysics(moveAmount / Time.deltaTime);
        velocity.y = 0;
        aiControl.Move(velocity * Time.deltaTime);
    }

    IEnumerator MoveToFirstPatrolPoint()
    {
        startingPatrol = true;
        Vector2 start = aiControl.transform.position;
        Vector2 end = patrolPoints[fromPatrolPointIndex];
        float dist = Mathf.Abs(start.x - end.x);
        float percent = 0f;

        while(percent < 1)
        {
            Debug.Log(percent);
            Vector2 newPos = LerpBetweenPoints(start, end, ref percent, dist);
            MovePatroller(newPos - (Vector2) aiControl.transform.position);
            yield return new WaitForEndOfFrame();
        }

        percentBetweenPatrolPoints = 0f;
        startingPatrol = false;
        patrolStarted = true;
    }

    Vector2 LerpBetweenPoints(Vector2 start, Vector2 end, ref float percent, float distance)
    {
        percent += Time.deltaTime * speed / distance;
        percent = Mathf.Clamp01(percent);
        Vector2 newPos = Vector2.Lerp(start, end, Ease(percent));
        newPos.y = end.y;
        return newPos;
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
            Vector2 newPos = LerpBetweenPoints(
                patrolPoints[fromPatrolPointIndex],
                patrolPoints[toPatrolPointIndex],
                ref percentBetweenPatrolPoints,
                distanceBetweenPatrolPoints);

            return newPos - (Vector2)aiControl.transform.position;
        }
        else//Reset
        {
            percentBetweenPatrolPoints = 0f;
            fromPatrolPointIndex = (fromPatrolPointIndex + 1) % patrolPoints.Length;
            toPatrolPointIndex = (fromPatrolPointIndex + 1) % patrolPoints.Length;//+1
            distanceBetweenPatrolPoints = Mathf.Abs(patrolPoints[fromPatrolPointIndex].x - patrolPoints[toPatrolPointIndex].x);

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
