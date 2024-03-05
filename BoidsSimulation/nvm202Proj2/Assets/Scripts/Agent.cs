using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

//Base class of agent objects
public abstract class Agent : MonoBehaviour
{
    [SerializeField]
    protected PhysicsObject myPhysicsObject;

    /// <summary>
    /// gets the physics object of the agent
    /// </summary>
    public PhysicsObject MyPhysicsObject { get { return myPhysicsObject; } }

    [SerializeField]
    protected float maxForce = 10;

    [SerializeField]
    float perlinValue = 0.1f;
    float perlinOffset;

    protected Vector3 wanderTarget;

    float wanderAngle;

    protected Vector3 totalForce;

    [SerializeField]
    [Range(0.1f, 5.0f)]
    float futureTime = 0.5f;

    [SerializeField]
    protected AgentManager manager;

    [SerializeField]
    SpriteRenderer spriteRenderer;

    /// <summary>
    /// gets the fleer's sprite renderer
    /// </summary>
    public SpriteRenderer SpriteRenderer { get { return spriteRenderer; } }

    //strength of separation
    [SerializeField]
    private float separationRange = 1;

    FlockManager flockManager;

    /// <summary>
    /// gets the agent's flock manager
    /// </summary>
    protected FlockManager FlockManager { get { return flockManager; } }

    /// <summary>
    /// sets the agent manager
    /// </summary>
    public AgentManager Manager { set { manager = value; } }

    //list of obstacles in the way of agent
    protected List<Vector3> foundObstacles = new List<Vector3>();

    // Start is called before the first frame update
    void Start()
    {
        //wanderAngle = Random.Range(0, Mathf.PI * 2);
        perlinOffset = Random.Range(0, 1000);
    }

    // Update is called once per frame
    void Update()
    {
        totalForce = Vector3.zero;
        CalcSteeringForces();
        totalForce = Vector3.ClampMagnitude(totalForce, maxForce);
        myPhysicsObject.ApplyForce(totalForce);
        transform.rotation = Quaternion.LookRotation(Vector3.forward, myPhysicsObject.Velocity.normalized);
    }

    /// <summary>
    /// calculate the forces to 
    /// simulate desired behavior
    /// </summary>
    protected abstract void CalcSteeringForces();

    /// <summary>
    /// seeks out the target position
    /// </summary>
    /// <param name="targetPos">position to seek</param>
    /// <returns>the steering force 
    /// towards the target</returns>
    protected Vector3 Seek(Vector3 targetPos)
    {
        Vector3 desiredVel = targetPos - transform.position;
        //velocity of zero or max speed
        desiredVel = desiredVel.normalized * myPhysicsObject.MaxSpeed;
        return desiredVel - myPhysicsObject.Velocity;
    }

    /// <summary>
    /// grabs a targets position and returns 
    /// the steering force towards it
    /// </summary>
    /// <param name="target">target obj to seek</param>
    /// <returns>steering force towards the object</returns>
    protected Vector3 Seek(GameObject target)
    {
        return Seek(target.transform.position);
    }

    /// <summary>
    /// flees from the tragets position
    /// </summary>
    /// <param name="targetPos">target obj to seek</param>
    /// <returns>steering force to flee from target</returns>
    protected Vector3 Flee(Vector3 targetPos)
    {
        Vector3 desiredVel = transform.position - targetPos;
        //velocity of zero or max speed
        desiredVel = desiredVel.normalized * myPhysicsObject.MaxSpeed;
        return desiredVel - myPhysicsObject.Velocity;
    }

    /// <summary>
    /// calculates the fleeing force
    /// </summary>
    /// <param name="target">target game object</param>
    /// <returns>the steering force</returns>
    protected Vector3 Flee(GameObject target)
    {
        return Flee(target.transform.position);
    }

    /// <summary>
    /// handles collisions between two obj
    /// </summary>
    /// <param name="target"></param>
    /// <param name="fleer"></param>
    protected void CollisionHandler(PhysicsObject target, PhysicsObject fleer)
    {
        if (CollisionCheck(target, fleer))
        {
            Camera mainCam = Camera.main;
            float camHeight = mainCam.orthographicSize;
            float totalCamHeight = camHeight * 2f;
            float totalCamWidth = totalCamHeight * mainCam.aspect;
            float camWidth = totalCamWidth / 2f;

            fleer.SetPosition(new Vector3(Random.Range(-camWidth, camWidth),
                              Random.Range(-camHeight, camHeight)));
        }
    }

    /// <summary>
    /// checks if two objects have collidede
    /// </summary>
    /// <param name="target">target phys obj</param>
    /// <param name="fleer">fleer phys obj</param>
    /// <returns></returns>
    private bool CollisionCheck(PhysicsObject target, PhysicsObject fleer)
    {
        Vector2 targetPos = target.transform.position;
        Vector2 fleerPos = fleer.transform.position;

        float distanceBetweenObjectsSquared =
            (fleerPos.x - targetPos.x) * (fleerPos.x - targetPos.x) +
            (fleerPos.y - targetPos.y) * (fleerPos.y - targetPos.y);

        if (distanceBetweenObjectsSquared >
       (target.Radius + fleer.Radius) * (target.Radius + fleer.Radius))
        {
            return false;
        }
        return true;
    }
    /// <summary>
    /// calculates a point to wander towards
    /// </summary>
    /// <param name="time">time ahead we are predicting</param>
    /// <param name="radius">radius of the point we are trying to wander towards</param>
    /// <returns>the steering force</returns>
    protected Vector3 Wander(float time, float radius)
    {
        //pick a distance ahead
        Vector3 futurePos = CalcFuturePosition(time);
        //imagine a circle there
        //pick a random angle
        wanderAngle += (0.5f - (Mathf.PerlinNoise(transform.position.x * perlinValue + perlinOffset,
                                        transform.position.y * perlinValue + perlinOffset)))
                                        * Mathf.PI
                                        * Time.deltaTime;
        //use that to get a point on the circle
        Vector3 targetPos = new Vector3(Mathf.Cos(wanderAngle),
                                        Mathf.Sin(wanderAngle)) * radius;


        wanderTarget = futurePos + targetPos;
        //returns the steering force, not the desired velocity
        return Seek(futurePos + targetPos);
    }

    /// <summary>
    /// predicts the future position, 
    /// doesnt account for steering forces
    /// </summary>
    /// <param name="time">time ahead we are predicting</param>
    /// <returns>future position</returns>
    protected Vector3 CalcFuturePosition(float time)
    {
        return transform.position + (myPhysicsObject.Velocity * time);
    }

    /// <summary>
    /// calculates a force to stay in bounds
    /// </summary>
    /// <returns>a force to return to screen bounds 
    /// if outside them, Vector3.zero otherwise</returns>
    protected Vector3 StayInBoundsForce()
    {
        Vector3 futurePos = CalcFuturePosition(futureTime);

        if (futurePos.x > myPhysicsObject.ScreenMax.x ||
            futurePos.x < myPhysicsObject.ScreenMin.x ||
            futurePos.y > myPhysicsObject.ScreenMax.y ||
            futurePos.y < myPhysicsObject.ScreenMin.y)
        {
            Vector3 cameraPos = Camera.main.transform.position;
            //prevents moving in z direction
            cameraPos.z = transform.position.z;
            return Seek(cameraPos);
        }
        return Vector3.zero;
    }

    /// <summary>
    /// flees from every agent in the 
    /// scene to create a separation effect
    /// proportional to the distance between 
    /// this and the obj to flee from
    /// </summary>
    /// <returns>force to generate a separation effect</returns>
    protected Vector3 Separate()
    {
        Vector3 separateForce = Vector3.zero;
        foreach (Agent agent in manager.Agents)
        {
            if (agent != this)
            {
                //add small distance to prevent divide by zero
                separateForce += Flee(agent.transform.position)
                    * (separationRange / (Vector3.Distance(transform.position, agent.transform.position) + .0001f));
            }
        }
        return separateForce;
    }

    /// <summary>
    /// separates agent from the specified list
    /// </summary>
    /// <param name="agents">list of agents to separate from</param>
    /// <returns>separation steering force</returns>
    protected Vector3 SeparateFleerAndWanders(List<Agent> agents)
    {
        Vector3 separateForce = Vector3.zero;
        foreach (Agent agent in agents)
        {
            if (agent != this)
            {
                //add small distance to prevent divide by zero
                separateForce += Flee(agent.transform.position)
                    * (separationRange / (Vector3.Distance(transform.position, agent.transform.position) + .0001f));
            }
        }
        return separateForce;
    }


    /// <summary>
    /// finds the closest agent
    /// </summary>
    /// <returns>the closest agent, or null if there is no other agents in the agents list</returns>
    public Agent FindClosest()
    {
        float minDist = float.MaxValue;
        Agent closest = null;

        foreach (Agent agent in manager.Agents)
        {
            if (agent != this)
            {
                float distance = Vector2.Distance(transform.position, agent.transform.position);
                if (distance < minDist)
                {
                    minDist = distance;
                    closest = agent;
                }
            }
        }
        return closest;
    }

    public TagPlayer FindClosestTagPlayer()
    {
        float minDist = float.MaxValue;
        TagPlayer closest = null;

        foreach (TagPlayer player in manager.TagPlayers)
        {
            if (player != this)
            {
                float distance = Vector2.Distance(transform.position, player.transform.position);
                if (player.CurrentState == TagStates.NotIt && distance < minDist)
                {
                    minDist = distance;
                    closest = player;
                }
            }
        }
        return closest;
    }

    /// <summary>
    /// creates a flocking effect
    /// </summary>
    /// <returns>steering force to reach the center of the flock</returns>
    protected Vector3 Cohesion()
    {
        return Seek(flockManager.transform.position);
    }

    /// <summary>
    /// aligns flock member with the center of the flock
    /// </summary>
    /// <returns>velocity needed to go towards the center of the flock</returns>
    protected Vector3 Alignment()
    {
        Vector3 desiredVelocity = flockManager.SharedDirection *
            myPhysicsObject.MaxSpeed;

        return desiredVelocity - myPhysicsObject.Velocity;
    }

    protected Vector3 AvoidObstacles(float avoidTime)
    {
        Vector3 avoidForce = Vector3.zero;
        foundObstacles.Clear();

        Vector3 futurePosition = CalcFuturePosition(avoidTime);
        float maxDistance = Vector3.Distance(transform.position, futurePosition) + myPhysicsObject.Radius;

        //Detect and avoid obstacles
        foreach (RotateObstacle obs in manager.Obstacles)
        {
            //calculate forward dot product
            Vector3 agentToObs = obs.transform.position - transform.position;
            //transform.up is forward vector
            //projecting agent to obs onto the forward vector
            float forwardDot = Vector3.Dot(agentToObs, transform.up);
            //project agent to obs onto right vector
            float rightDot = Vector3.Dot(agentToObs, transform.right);

            if (forwardDot >= -obs.Radius &&
                forwardDot <= (maxDistance + obs.Radius) &&
                Mathf.Abs(rightDot) <= (myPhysicsObject.Radius + obs.Radius))
            {
                foundObstacles.Add(obs.transform.position);

                if (rightDot > 0)
                {
                    //go left if obstacle is to right
                    //scale by distance by dividing maxDistance by forward dot
                    //max - fdot
                    avoidForce += transform.right * (-(maxDistance - forwardDot) / maxDistance);
                }
                else
                {
                    //if obstacle is to the left or in front, go right
                    avoidForce += transform.right * ((maxDistance - forwardDot) / maxDistance);
                }
            }
        }

        return avoidForce;
    }

    /// <summary>
    /// finds the closest flock manager to the agent
    /// </summary>
    /// <returns>the FlockManager</returns>
    protected void FindClosestFlockManager()
    {
        FlockManager closest = null;
        float minDist = float.MaxValue;

        if (flockManager != null)
        {
            flockManager.flock.Remove(this);
        }

        foreach (FlockManager manager in manager.FlockManagers)
        {
            if (manager != this.flockManager)
            {
                float distance = Vector2.Distance(transform.position, manager.transform.position);
                if (distance < minDist)
                {
                    minDist = distance;
                    closest = manager;
                }
            }
        }
        flockManager = closest;
        flockManager.flock.Add(this);
    }
}
