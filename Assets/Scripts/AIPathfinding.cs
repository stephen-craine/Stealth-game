using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;


//To do: 
    //stop two AI at equal distances inspecting the same point.
    //if investigating a spot, and another spot is triggered, ignore?
    //breaking line of sight i.e. work on chase state
    //add cyan lighting for a time and speed up until moving back to not suspicious
    //use weighting of waypoints to coordinate a smart patrol state
    //add a scoring and winning the game

public class AIPathfinding : MonoBehaviour {

    //Spotlight colours: red- chase, yellow- investigate, cyan- suspicious, white- normal(patrolling), magenta- smart patrol
    
    [SerializeField] //allows viewing/editing of AIDestination in the Unity editor
    ConnectedWaypoint _currentWaypoint;
    public ConnectedWaypoint myWaypoint;
    ConnectedWaypoint _previousWaypoint;
    NavMeshAgent agent;
    public float patrolSpeed = 7f;
    public float chaseSpeed = 9f;
    Transform player;
    public float pathLength;
    public float waiting;
    public float waitTimer;
    public bool waitingAgent;
   [SerializeField] Dictionary<GameObject, bool> triggerDict = new Dictionary<GameObject, bool>(); //dict of triggers which will be deleted from here when checked



    //enemy sight
    public Light spotlight;
    public float viewDistance;
    float viewAngle;
    public LayerMask viewMask;
    Color spotLightOriginalColor;


    //FSM initial setup
    public enum State
    {
        PATROL,
        CHASE,
        INVESTIGATE //1 AI max can be in this state
            //suspicious
            //SMART PATROL
    }
    public State state;
    private bool checkedA;
    private bool alive;
    public Transform placeToCheck;

    //pathfinding initial setup
    [SerializeField] int waypointsVisited;
    
    [SerializeField] bool travelling;


    void Start() {
        waitingAgent = false;
        player = GameObject.FindGameObjectWithTag("Player").transform; // player transform
        spotLightOriginalColor = spotlight.color;
        viewAngle = spotlight.spotAngle;
        waypointsVisited = 0;

       
        GameObject[] triggerList = GameObject.FindGameObjectsWithTag("Trigger");
        foreach(GameObject i in triggerList)
        {
            triggerDict.Add(i, false);  //populating dictionary with all triggers from game
        }

        agent = this.GetComponent<NavMeshAgent>();
        
     
        //FSM
        state = AIPathfinding.State.PATROL;

        alive = true;

        StartCoroutine("FSM");
    }
    IEnumerator FSM()
    {
        while (alive)
        {
            switch (state)
            {
                case State.PATROL:
                    Patrol();
                    break;
                case State.CHASE:
                    Chase();
                    break;
                case State.INVESTIGATE:
                    Investigate();
                    break;
            }
            yield return null;
        }
    }

    void Chase()
    {
        agent.speed = chaseSpeed;
        agent.SetDestination(player.position);

    }

    public void Investigate()
    {
        Debug.Log("Investigate");
        waitingAgent = false;
        this.spotlight.color = Color.yellow;
        waiting = 0f;
        waitTimer = 2f;
        Vector3 checkVector = placeToCheck.position;
        agent.SetDestination(checkVector);
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
            {
                waitingAgent = true;
            Debug.Log("HELLO");
        }

        while(waitingAgent == true)
        {
            if (waiting < waitTimer)
            {
                waiting += 1f;
            }
            else if(waiting == waitTimer)
            {
                state = AIPathfinding.State.PATROL;
                waitingAgent = false;
                this.spotlight.color = Color.cyan;
            }
        }
        
    }

    private void OnTriggerEnter(Collider other)
    {
       // NEED TO GIVE GAME OVER OR LOSE HEALTH
        if (other.tag == "Player")
        {
            SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
        }
    }

    void Patrol() {
        agent.speed = patrolSpeed;
        if (agent == null)
        {
            Debug.LogError("Nav mesh agent component not attached to: " + gameObject.name);
        }
        else
        {
            if (_currentWaypoint == null) //setting initial waypoint
            {
                GameObject[] allWaypoints = GameObject.FindGameObjectsWithTag("Waypoint");

                while (_currentWaypoint == null)
                {
                    int random = UnityEngine.Random.Range(0, allWaypoints.Length);
                    ConnectedWaypoint initialWaypoint = allWaypoints[random].GetComponent<ConnectedWaypoint>();
                    _currentWaypoint = initialWaypoint;
                }
            }
            if (!travelling)
            {
                SetDestination();
            }
        }
    }

    public void SetDestination()
    {
        if (waypointsVisited > 0)
        {
            if (_previousWaypoint == null)
            {
                _previousWaypoint = _currentWaypoint;
            }
            ConnectedWaypoint nextWaypoint = _currentWaypoint.NextWaypoint(_previousWaypoint); //gets the next waypoint from the ConnectedWaypoint
            _previousWaypoint = _currentWaypoint;
            _currentWaypoint = nextWaypoint;
        }

        Vector3 targetVector = _currentWaypoint.transform.position;
        myWaypoint = _currentWaypoint;
        myWaypoint.GetComponent<ConnectedWaypoint>().WeightWaypoint(); //update visiting and weight of waypoint
        agent.SetDestination(targetVector);
        travelling = true;

    }

    public void Update()
    {
        if (travelling && agent.remainingDistance <= 1.0f && state == State.PATROL)
        {
            myWaypoint.GetComponent<ConnectedWaypoint>().beingVisited = false;
            travelling = false;
            waypointsVisited++;
        }

        if (SpotPlayer())
        {
            spotlight.color = Color.red;
            state = AIPathfinding.State.CHASE; //need to change this so only 1 AI actively chases and the others go into SMART patrol mode
            
        }
        else
        {
            if (spotlight.color == Color.red)
            {
                spotlight.color = spotLightOriginalColor;
            }
        }
        if (triggerDict != null)
        {
            List<GameObject> keys = new List<GameObject>(triggerDict.Keys);
            foreach (GameObject s in keys)
            {

                if (s.GetComponent<CreateCollectible>().collected == true && triggerDict[s] == false)
                {
                    placeToCheck = s.GetComponent<CreateCollectible>().Spawnpoint;
                    triggerDict[s] = true;
                    checkPath(agent.transform, placeToCheck);
                }
            }
        }
    }

    void checkPath(Transform agentPosition, Transform triggerLoc)
    {
        Debug.Log("Path calculating");
        pathLength = Vector3.Distance(agentPosition.position, triggerLoc.position);
        //Trigger Investigate state for closest guard AI- look at other guards to see if anyone is closer
        //compare my distance to all other distances for other guards, if I am the smallest, investigate, if not carry on with patrol but suspicious state.
        GameObject[] guardList = GameObject.FindGameObjectsWithTag("Guard");  //the items
        float[] pathList = new float[guardList.Length]; //the keys
            
        for(int i = 0; i < guardList.Length; i++)
        {
            pathList[i] = guardList[i].GetComponent<AIPathfinding>().pathLength;
        }
        Array.Sort(pathList, guardList); //sorts guard list into ascending path lengths

        if (guardList[0].GetComponent<AIPathfinding>().state != State.CHASE) //if the closest guard is not chasing, investigate
        {
            if (this.gameObject == guardList[0]) //only change state if this guard is the one
            {


                guardList[0].GetComponent<AIPathfinding>().state = State.INVESTIGATE;
            }
        }
            foreach(GameObject guard in guardList)
            {
                if (guard.GetComponent<AIPathfinding>().state == State.PATROL){
                //if a guard is patrolling and not sent to investigate/chase then switch to smart patrol or suspicious
                guard.GetComponent<AIPathfinding>().spotlight.color = Color.cyan;
                }
            }
            
         
    }

    bool SpotPlayer()
    {
        if (Vector3.Distance(transform.position, player.position) < viewDistance)
        {
            Vector3 dirToPlayer = (player.position - transform.position).normalized;
            float angleBetweenGuardPlayer = Vector3.Angle(transform.forward, dirToPlayer);
            if (angleBetweenGuardPlayer < viewAngle / 2f)
            {
                if(!Physics.Linecast(transform.position, player.position, viewMask))
                {
                    return true;
                }
            }
        }
        return false;
    }

        }