using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FleeAndWander : Agent
{
    [SerializeField]
    GameObject target;

    [SerializeField]
    [Range(0.1f, 10f)]
    float avoidanceTime;

    [SerializeField]
    [Range(0f, 100f)]
    float avoidWeight;

    /// <summary>
    /// sets the targeted game object
    /// </summary>
    public GameObject Target { set { target = value; } }

    [SerializeField]
    PhysicsObject targetPhys;

    /// <summary>
    /// sets the targeted phys object
    /// </summary>
    public PhysicsObject TargetPhys { set { targetPhys = value; } }

    private Vector3 fleeForce;

    [SerializeField]
    [Range(0f, 100.0f)]
    private float boundsWeight;

    [SerializeField]
    [Range(0f, 10f)]
    float wanderTime = 1f;

    [SerializeField]
    [Range (0f, 5f)]
    float wanderRadius = 1f;

    protected override void CalcSteeringForces()
    {
        totalForce += StayInBoundsForce() * boundsWeight;
        totalForce += Flee(FindClosest().transform.position);
        totalForce += Wander(wanderTime, wanderRadius);
        totalForce += SeparateFleerAndWanders(manager.FleeAndWanderers);
        totalForce += Separate();
        totalForce += AvoidObstacles(avoidanceTime) * avoidWeight;
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
