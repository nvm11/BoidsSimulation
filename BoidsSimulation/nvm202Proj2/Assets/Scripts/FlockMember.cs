using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum FlockerStates
{
    Fleeing,
    Flocking
}

public class FlockMember : Agent
{
    [SerializeField]
    float fleeTime = 1f;

    float timer = 0f;

    [SerializeField]
    [Range(0.0f, 25.0f)]
    float boundsWeight = 1.0f;

    [SerializeField]
    [Range(0.0f, 25.0f)]
    float separationWeight = 1.0f;

    [SerializeField]
    [Range(0.0f, 25.0f)]
    float cohesionWeight = 1.0f;

    [SerializeField]
    [Range(0.0f, 25.0f)]
    float alignmentWeight = 1.0f;

    [SerializeField]
    float avoidTime = 1.0f;

    FlockerStates currentState;

    /// <summary>
    /// gets the current state of the FlockingMember
    /// </summary>
    public FlockerStates CurrentState { get { return currentState; } }

    protected override void CalcSteeringForces()
    {
        totalForce += Separate() * separationWeight;
        totalForce += StayInBoundsForce() * boundsWeight;

        switch (currentState)
        {
            case FlockerStates.Flocking:
                totalForce += Cohesion() * cohesionWeight;
                totalForce += Alignment() * alignmentWeight;
                break;

            case FlockerStates.Fleeing:
                timer += Time.deltaTime;
                FindClosestFlockManager();
                if (FlockManager != null)
                {
                    Flee(FlockManager.transform.position);

                    if (timer >= fleeTime)
                    {
                        SetState(FlockerStates.Flocking);
                    }
                }
                break;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Vector3 futurePosition = CalcFuturePosition(avoidTime);

        float boxHeight = Vector3.Distance(transform.position, futurePosition) +
            myPhysicsObject.Radius;

        Vector3 boxSize = new Vector3(
            myPhysicsObject.Radius * 2,
            boxHeight,
            myPhysicsObject.Radius * 2
        );

        Vector3 boxCenter = new Vector3(0, boxHeight / 2, 0);

        Gizmos.color = Color.green;

        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawWireCube(boxCenter, boxSize);
        Gizmos.matrix = Matrix4x4.identity;
    }

    /// <summary>
    /// sets the state of the flock member
    /// </summary>
    /// <param name="newState">state to be assigned</param>
    public void SetState(FlockerStates newState)
    {
        currentState = newState;

        if (newState == FlockerStates.Fleeing)
        {
            SpriteRenderer.color = Color.magenta;
        }
        else
        {
            SpriteRenderer.color = FlockManager.SpriteRenderer.color;
            timer = 0f;
        }
    }
}
