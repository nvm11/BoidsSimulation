using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class AgentManager : MonoBehaviour
{
    //[SerializeField]
    //Wanderer wanderPrefab;

    //[SerializeField]
    //Seeker seekPrefab;

    [SerializeField]
    FleeAndWander fleePrefab;

    [SerializeField]
    TagPlayer tagPrefab;

    List<Agent> agents;

    List<TagPlayer> tagPlayers = new List<TagPlayer>();

    List<Agent> fleerAndWanderers;

    /// <summary>
    /// gets the list of fleer and wanderers
    /// </summary>
    public List<Agent> FleeAndWanderers { get { return fleerAndWanderers; } }

    [SerializeField]
    [Range(0f, 10.0f)]
    float countTimer;

    List<TagPlayer> itPlayers = new List<TagPlayer>();

    [SerializeField]
    RotateObstacle obstaclePrefab;

    List<RotateObstacle> obstacles;

    public List<RotateObstacle> Obstacles { get { return obstacles; } }

    [SerializeField]
    Color[] fleerColors = new Color[5];

    /// <summary>
    /// gets the time of how long to wait before transitioning between states
    /// </summary>
    public float CountTimer { get { return countTimer; } }


    /// <summary>
    /// gets the list of agents
    /// </summary>
    public List<Agent> Agents { get { return agents; } }

    //desired number of wanderers
    [SerializeField]
    [Range(0, 10)]
    uint numWanderers;

    [SerializeField]
    [Range(0, 15)]
    uint numObstacles = 5;

    [SerializeField]
    [Range(0, 10)]
    uint numSquares = 5;

    [SerializeField]
    [Range(0, 10)]
    uint numTagPlayers = 5;

    [SerializeField]
    [Range(0, 20)]
    uint numFlockMembers = 10;

    /// <summary>
    /// gets the number of tag players in the scene
    /// </summary>
    public uint NumTagPlayers { get { return numTagPlayers; } }

    public List<TagPlayer> TagPlayers { get { return tagPlayers; } }

    /// <summary>
    /// gets or seysthe player that is it
    /// </summary>
    public List<TagPlayer> ItPlayers { get { return itPlayers; } }

    [SerializeField]
    FlockMember FlockMemberPrefab;

    List<FlockMember> flockMembers = new List<FlockMember>();

    /// <summary>
    /// gets the list of flock memebers
    /// </summary>
    public List<FlockMember> FlockMembers { get { return flockMembers; } }

    List<FlockManager> flockManagers = new List<FlockManager>();

    public List<FlockManager> FlockManagers { get { return flockManagers; } }

    // Start is called before the first frame update
    void Start()
    {
        agents = new List<Agent>();
        obstacles = new List<RotateObstacle>();
        fleerAndWanderers = new List<Agent>();

        for (uint i = 0; i < numObstacles; i++)
        {
            SpawnObstacle();
        }

        for (uint i = 0; i < numTagPlayers; i++)
        {
            SpawnTagPlayer();
        }

        for (uint i = 0; i < numSquares; i++)
        {
            SpawnFleerAndWanderer();
        }

        for (uint i = 0; i < numFlockMembers; i++)
        {
            SpwanFlockMember();
        }

        if (agents.Count > 0)
        {
            tagPlayers[0].SetState(TagStates.Counting);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (numTagPlayers == itPlayers.Count)
        {
            ResetStates();
        }
    }

    ///// <summary>
    ///// creates a wanderer, 
    ///// makes it a child of the manager, 
    ///// and adds it to the agent list
    ///// </summary>
    //private void SpawnWanderer()
    //{
    //    //parents the prefab as a child of this manager
    //    agents.Add(Instantiate(wanderPrefab, transform));
    //    agents[agents.Count - 1].Manager = this;
    //}

    ///// <summary>
    ///// creates a seeker,
    ///// makes it a child of the manager
    ///// and adds it to the agent list
    ///// </summary>
    //private void SpawnSeeker()
    //{
    //    agents.Add(Instantiate(seekPrefab, transform));
    //    agents[agents.Count - 1].Manager = this;
    //}

    /// <summary>
    /// creates a fleer,
    /// makes it a child of the manager,
    /// and adds it to the agent list
    /// </summary>
    private void SpawnFleerAndWanderer()
    {
        FleeAndWander fleer = Instantiate(fleePrefab, transform);
        fleer.SpriteRenderer.color = fleerColors[UnityEngine.Random.Range(0, fleerColors.Length)];
        fleer.Manager = this;
        fleerAndWanderers.Add(fleer);
        UpdateTarget(fleer);
    }


    /// <summary>
    /// updates the fleer's target properties
    /// </summary>
    /// <param name="fleer">fleer to update</param>
    private void UpdateTarget(FleeAndWander fleer)
    {
        Agent agent = fleer.FindClosest();
        fleer.Target = agent.gameObject;
        fleer.TargetPhys = agent.MyPhysicsObject;
    }

    /// <summary>
    /// spawns a tag player
    /// </summary>
    private void SpawnTagPlayer()
    {
        //tagplayer is null
        TagPlayer tagPlayer = Instantiate(tagPrefab, transform);
        tagPlayer.Manager = this;
        agents.Add(tagPlayer);
        tagPlayers.Add(tagPlayer);
        flockManagers.Add(tagPlayer.FlockingScript);
    }

    /// <summary>
    /// spawns an obstacle in a random position on the screen
    /// use this at Start(), nowhere else
    /// </summary>
    private void SpawnObstacle()
    {
        RotateObstacle obstacle = Instantiate(obstaclePrefab, transform);
        obstacle.transform.position = new Vector2(UnityEngine.Random.Range(obstacle.PhysicsObject.ScreenMin.x, obstacle.PhysicsObject.ScreenMax.x),
                                                  UnityEngine.Random.Range(obstacle.PhysicsObject.ScreenMin.y, obstacle.PhysicsObject.ScreenMax.y));
        obstacles.Add(obstacle);
    }

    /// <summary>
    /// spawns an obstacle at the specified position and 
    /// removes the oldest obstacle from the queue
    /// </summary>
    /// <param name="newPos">new position for the obstacle</param>
    private void SpawnObstacle(Vector2 newPos)
    {
        RotateObstacle obstacle = Instantiate(obstaclePrefab, transform);
        obstacle.transform.position = newPos;
        obstacles.Add(obstacle);
        Destroy(obstacles[0].gameObject);
        obstacles.RemoveAt(0);
    }

    /// <summary>
    /// spawns a flock member at the origin
    /// </summary>
    private void SpwanFlockMember()
    {
        FlockMember member = Instantiate(FlockMemberPrefab, transform);
        member.Manager = this;
        agents.Add(member);
        flockMembers.Add(member);
    }

    /// <summary>
    /// creates an obstacle at the mouse position
    /// </summary>
    /// <param name="context">fire press</param>
    public void OnFire(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            SpawnObstacle(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        }
    }

    /// <summary>
    /// resets the list of tag players
    /// </summary>
    private void ResetStates()
    {
        //make flockers flee
        foreach (FlockMember flocker in flockMembers)
        {
            flocker.SetState(FlockerStates.Fleeing);
        }

        //reset tag players
        foreach (TagPlayer player in tagPlayers)
        {
            player.SetState(TagStates.NotIt);
        }
        itPlayers.Clear();
        tagPlayers[0].SetState(TagStates.Counting);
        itPlayers.Add(tagPlayers[0]);
    }
}
