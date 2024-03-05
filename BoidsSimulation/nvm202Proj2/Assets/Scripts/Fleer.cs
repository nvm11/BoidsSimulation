using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fleer : Agent
{
    [SerializeField]
    GameObject target;

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

    /// <summary>
    /// calculates forces to steer agnet
    /// </summary>
    protected override void CalcSteeringForces()
    {
        fleeForce = Flee(target);
        totalForce += fleeForce;

        CollisionHandler(targetPhys, myPhysicsObject);
    }

    /// <summary>
    /// draws the velocity of the seeker 
    /// and seek force if selected
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawLine(transform.position, myPhysicsObject.Velocity);

        Gizmos.color = Color.red;
        //force is scaled down so it doesnt go off screen
        Gizmos.DrawLine(transform.position, fleeForce * 0.3f);

    }
}
