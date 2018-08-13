using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class AIPathfinding : MonoBehaviour {

    [SerializeField] //allows viewing/editing of AIDestination in the Unity editor
    ConnectedWaypoint _currentWaypoint;
    public ConnectedWaypoint myWaypoint;
    ConnectedWaypoint _previousWaypoint;
    NavMeshAgent agent;
    public float patrolSpeed = 7f;
    public float chaseSpeed = 9f;
    Transform player;
   [SerializeField] Dictionary<GameObject, bool> triggerDict = new Dictionary<GameObject, bool>(); //dict of triggers which will be deleted from here when checked
    //CreateCollectible hasCollectedA;


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
        INVESTIGATE
    }
    public State state;
    private bool checkedA;
    private bool alive;
    Transform placeToCheck;

    //pathfinding initial setup
    [SerializeField] int waypointsVisited;
    
    [SerializeField] bool travelling;

    //[SerializeField] bool _waiting;

    //float waitTimer;

    //[SerializeField] float waitTime = 1f;







    void Start() {
        player = GameObject.FindGameObjectWithTag("Player").transform; // player transform
        spotLightOriginalColor = spotlight.color;
        viewAngle = spotlight.spotAngle;
        //_waiting = false;
        waypointsVisited = 0;

       
        GameObject[] triggerList = GameObject.FindGameObjectsWithTag("Trigger");
        foreach(GameObject i in triggerList)
        {
            triggerDict.Add(i, false);  //populating dictionary with all triggers from game
        }

        agent = this.GetComponent<NavMeshAgent>();
        

        //if((hasCollectedA == null) && (GameObject.FindGameObjectWithTag("Trigger").GetComponent<CreateCollectible>() != null)){
        //    hasCollectedA = GameObject.FindGameObjectWithTag("Trigger").GetComponent<CreateCollectible>();

        //} else
        //{
        //    Debug.LogError("Missing CreateCollectible script component");
        //}
        



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

    void Investigate()
    {
        agent.SetDestination(placeToCheck.transform.position);
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
                    //could add debug here in case of not finding a waypoint.
                }
            }
            if (!travelling)
            {
                SetDestination();
            }

        }

    }



    private void SetDestination()
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
        if (travelling && agent.remainingDistance <= 1.0f)
        {
            travelling = false;
            waypointsVisited++;
            //if (!_waiting) //if not already waiting, start waiting
            //{
            //    waitTimer = 0f;
            //    _waiting = true;
            //}
        }

      

        if (SpotPlayer())
        {
            spotlight.color = Color.red;
            state = AIPathfinding.State.CHASE;
            
        }
        else
        {
            spotlight.color = spotLightOriginalColor;
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
                    state = AIPathfinding.State.INVESTIGATE;
                }
            }
        }


        //if (_waiting) //if waiting keep waiting until time is up
        //{
        //    waitTimer += Time.deltaTime;
        //    if (waitTimer >= waitTime)
        //    {
        //        _waiting = false;
        //        SetDestination();
    }


    //    } 
    //}


    //void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.red;
    //    Gizmos.DrawRay(transform.position, transform.forward * viewDistance);
    //}

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