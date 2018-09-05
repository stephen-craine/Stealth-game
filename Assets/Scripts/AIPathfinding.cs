using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class AIPathfinding : MonoBehaviour {

    //Spotlight colours: red- chase, yellow- investigate, cyan- suspicious, white- normal(patrolling), magenta- smart patrol
    
    [SerializeField] //allows viewing/editing of AIDestination in the Unity editor
    public ConnectedWaypoint _currentWaypoint;
    public ConnectedWaypoint myWaypoint;
    ConnectedWaypoint _previousWaypoint;
    NavMeshAgent agent;
    public float patrolSpeed = 7f;
    public float chaseSpeed = 9f;
    Transform player;
    public ConnectedWaypoint initialWaypoint;
    public NavMeshPath path;
    public float pathLength;
    public float waiting;
    public float waitTimer;
    public bool waitingAgent;
    public string sectorName;
    public float chaseTimer;
    public bool enterPatrol;
   [SerializeField] Dictionary<GameObject, bool> triggerDict = new Dictionary<GameObject, bool>(); //dict of triggers which will be deleted from here when checked



    //enemy sight
    public Light spotlight;
    public float viewDistance;
    float viewAngle;
    public LayerMask viewMask;
    Color spotLightOriginalColor;
    public GameObject gameController;

    //FSM initial setup
    public enum State
    {
        PATROL,
        CHASE,
        INVESTIGATE 

    }
    public State state;
    private bool checkedA;
    private bool alive;
    Transform placeToCheck;

    //pathfinding initial setup
    [SerializeField] int waypointsVisited;
    
    [SerializeField] bool travelling;


    void Start() {
        gameController = GameObject.FindGameObjectWithTag("GameController");
        enterPatrol = true;
        chaseTimer = 0;
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
        while (gameController.GetComponent<GameDirector>().paused == true)
        {
            yield return null;
        }
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

    public void Chase()
    {
        waitingAgent = false;
        agent.speed = chaseSpeed;
        agent.SetDestination(player.position);
        chaseTimer += Time.deltaTime;
    }

    void Investigate()
    {
        waiting = 0;
        waitTimer = 2f;
        agent.SetDestination(placeToCheck.transform.position);
        if(agent.remainingDistance <= 1.0f)
        {
            waitingAgent = true;
        }
        while(waitingAgent == true && (waiting <= waitTimer))
        {
            if (waiting < waitTimer)
            {
                waiting += 1f;
            }
            else
            {
                state = AIPathfinding.State.PATROL;
                waitingAgent = false;
                this.spotlight.color = Color.cyan;
            }
        }
        
    }

    void OnTriggerEnter(Collider other)
    {
       // NEED TO GIVE GAME OVER OR LOSE HEALTH
        if (other.tag == "Player")
        {
            SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
        }

    }
    public void CheckMySector()
    {
        Collider[] hitColliders = Physics.OverlapSphere(this.transform.position, 2);
        int i = 0;
        while (i < hitColliders.Length)
        {
            Collider current = hitColliders[i];
            if (current.tag == "Sector")
            {
                sectorName = current.gameObject.name;
            }
            i++;
        }
    }


    public void Patrol() {
        
        

        if (enterPatrol == true)
        {
            
            Debug.Log("Patrol start");
            Debug.Log(this.gameObject);
            initialWaypoint = gameController.GetComponent<GameDirector>().initialDict[this.gameObject];
            _currentWaypoint = initialWaypoint;
            _previousWaypoint = initialWaypoint;
            enterPatrol = false;
        }
        agent.speed = patrolSpeed;
        waitingAgent = false;
        if (agent == null)
        {
            Debug.LogError("Nav mesh agent component not attached to: " + gameObject.name);
        }

        if (!travelling)
        {
            SetDestination();
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
        CheckMySector();
        if (travelling && agent.remainingDistance <= 1.0f)
        {
            myWaypoint.GetComponent<ConnectedWaypoint>().beingVisited = false;
            travelling = false;
            waypointsVisited++;
        }

        //if(GameDirector.State == GameDirector.State.NORMAL

        if (SpotPlayer())
        {
            this.state = State.CHASE;
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
                    path = new NavMeshPath();
                    agent.CalculatePath(placeToCheck.transform.position, path);
                    CheckPath(path);
                }
            }
        }
    }

    void CheckPath(NavMeshPath thisPath)
    {
        pathLength = thisPath.corners.Length;
        //Trigger Investigate state for closest guard AI- look at other guards to see if anyone is closer
        //compare my distance to all other distances for other guards, if I am the smallest, investigate, if not carry on with patrol but suspicious state.
        GameObject[] guardList = GameObject.FindGameObjectsWithTag("Guard");
        List<float> pathList = new List<float>();
            
        for(int i = 0; i < guardList.Length; i++)
        {
            pathList.Add(guardList[i].GetComponent<AIPathfinding>().pathLength);
        }
        pathList.Sort();
        if (this.pathLength == pathList[0]) //if this AI has the shortest path then investigate, otherwise don't
        {
            this.spotlight.color = Color.yellow;
            state = AIPathfinding.State.INVESTIGATE;
        } else
        {
            this.spotlight.color = Color.cyan;
        }
    }

    public bool SpotPlayer()
    {
        if (Vector3.Distance(transform.position, player.position) < viewDistance)
        {
            Vector3 dirToPlayer = (player.position - transform.position).normalized;
            float angleBetweenGuardPlayer = Vector3.Angle(transform.forward, dirToPlayer);
            if (angleBetweenGuardPlayer < viewAngle / 2f)
            {
                if(!Physics.Linecast(transform.position, player.position, viewMask))
                {
                    chaseTimer = 0; //whenever within cone of vision, reset chase timer
                    return true;
                }
            }
        }
        return false;
    }

        }