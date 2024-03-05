using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//seekign autonomous agent
public class Seeker : Agent
{
    [SerializeField]
    GameObject target;

    private Vector3 seekForce;

    /// <summary>
    /// 
    /// </summary>
    protected override void CalcSteeringForces()
    {
        seekForce = Seek(target);
        totalForce += seekForce;
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
        Gizmos.DrawLine (transform.position, seekForce * 0.3f);
        
    }
}
