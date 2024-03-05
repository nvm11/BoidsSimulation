using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wanderer : Agent
{
    [SerializeField]
    float wanderTime = 1f;

    [SerializeField]
    float wanderRadius = 1f;

    [SerializeField]
    [Range(0f, 100.0f)]
    float boundsWeight = 10.0f;

    protected override void CalcSteeringForces()
    {
        totalForce += Wander(wanderTime, wanderRadius);
        totalForce += StayInBoundsForce() * boundsWeight;
        totalForce += Separate();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(CalcFuturePosition(wanderTime), wanderRadius);
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, wanderTarget);
    }
}
