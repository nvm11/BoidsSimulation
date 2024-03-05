using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// states of tag players
/// </summary>
public enum TagStates
{
    NotIt,
    Counting,
    It
}

public class TagPlayer : Agent
{
    [SerializeField]
    float wanderTime = 1f;

    [SerializeField]
    float wanderRadius = 1f;

    [SerializeField]
    [Range(0f, 100.0f)]
    float boundsWeight = 10.0f;

    [SerializeField]
    [Range(0f, 100.0f)]
    float fleeWeight;

    //default is NotIt
    [SerializeField]
    TagStates currentState;
    float countTimer;

    TagPlayer target;

    [SerializeField]
    Color[] tagColors;

    [SerializeField]
    [Range(0.1f, 10f)]
    float avoidanceTime;

    [SerializeField]
    [Range(0f, 100f)]
    float avoidWeight;

    /// <summary>
    /// sets the tag player's current state
    /// </summary>
    public TagStates CurrentState { get { return currentState; } }

    /// <summary>
    /// gets Colors for tag players
    /// </summary>
    public Color[] TagColors { get { return tagColors; } }

    [SerializeField]
    FlockManager flockingScript;

    public FlockManager FlockingScript { get { return flockingScript; } }


    protected override void CalcSteeringForces()
    {
        totalForce += StayInBoundsForce() * boundsWeight;
        totalForce += AvoidObstacles(avoidanceTime) * avoidWeight;

        switch (currentState)
        {
            case TagStates.NotIt:
                totalForce += Wander(wanderTime, wanderRadius);
                totalForce += Separate();
                foreach (Agent itPlayer in manager.ItPlayers)
                {
                    totalForce += Flee(itPlayer.transform.position);
                }
                break;

            case TagStates.Counting:
                countTimer += Time.deltaTime;
                if (countTimer >= manager.CountTimer)
                {
                    countTimer = 0;
                    SetState(TagStates.It);
                }
                break;

            case TagStates.It:
                target = FindClosestTagPlayer();
                if (target == null) { break; }
                totalForce += Seek(target.transform.position);
                if (Vector2.Distance(transform.position, target.transform.position)
                    < myPhysicsObject.Radius + target.myPhysicsObject.Radius)
                {
                    target.SetState(TagStates.Counting);
                }
                break;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="newState"></param>
    public void SetState(TagStates newState)
    {
        currentState = newState;
        SpriteRenderer.color = tagColors[(int)currentState];
        if (newState == TagStates.It)
        {
            manager.ItPlayers.Add(this);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.black;
        Vector3 futurePostion = CalcFuturePosition(avoidanceTime);
        //distance from transform to radius of searching for obstacles
        float dist = Vector3.Distance(transform.position, futurePostion) + myPhysicsObject.Radius;
        Vector3 boxSize = new Vector3(myPhysicsObject.Radius * 2, dist, myPhysicsObject.Radius * 2);
        Vector3 boxCenter = new Vector3(0, dist / 2, 0);
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawWireCube(boxCenter, boxSize);
        Gizmos.matrix = Matrix4x4.identity;

        Gizmos.color = Color.red;
        foreach (Vector3 obstacle in foundObstacles)
        {
            Gizmos.DrawLine(transform.position, obstacle);
        }
    }
}
