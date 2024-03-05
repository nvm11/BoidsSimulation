using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateObstacle : MonoBehaviour
{
    [SerializeField]
    float rotateAmount = 20;


    [SerializeField]
    PhysicsObject physicsObject;

    /// <summary>
    /// gets the radius of the obstacle's physics object
    /// </summary>
    public float Radius
    {
        get { return physicsObject.Radius; }
    }

    /// <summary>
    /// gets the obstacle's physics object
    /// </summary>
    public PhysicsObject PhysicsObject { get { return physicsObject; } }

    // Update is called once per frame
    void Update()
    {
        transform.rotation *= Quaternion.Euler(0f, 0f, rotateAmount * Time.deltaTime);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, Radius);
    }
}
