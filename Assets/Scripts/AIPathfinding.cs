using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIPathfinding : MonoBehaviour {

    [SerializeField] //allows viewing/editing of AIDestination in the Unity editor
    ConnectedWaypoint _currentWaypoint;

    ConnectedWaypoint _previousWaypoint;
    NavMeshAgent navMeshAgent;

    [SerializeField]
    int waypointsVisited;

    [SerializeField]
    bool travelling;

    [SerializeField]
    bool _waiting;

    float waitTimer;

    [SerializeField]
    float waitTime = 1f;

    //[SerializeField]
    //float changeDirection = 0.1f; //probability of returning to previous waypoint

	void Start () {
        _waiting = false;
        waypointsVisited = 0;
        navMeshAgent = this.GetComponent<NavMeshAgent>();

        if(navMeshAgent == null)
        {
            Debug.LogError("Nav mesh agent component not attached to: " + gameObject.name);
        }
        else
        {
            if (_currentWaypoint == null) //setting initial waypoint
            {
                GameObject[] allWaypoints = GameObject.FindGameObjectsWithTag("Waypoint");

                while(_currentWaypoint == null)
                {
                    int random = UnityEngine.Random.Range(0, allWaypoints.Length);
                    ConnectedWaypoint initialWaypoint = allWaypoints[random].GetComponent<ConnectedWaypoint>();
                    _currentWaypoint = initialWaypoint;
                    //could add debug here in case of not finding a waypoint.
                }
            }
            SetDestination();  
        
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
            navMeshAgent.SetDestination(targetVector);
            travelling = true;
        
    }

    public void Update()
    {
        if (travelling && navMeshAgent.remainingDistance <= 1.0f)
        {
            travelling = false;
            waypointsVisited++;
            if (!_waiting) //if not already waiting, start waiting
            {
                waitTimer = 0f;
                _waiting = true;
            }
        }

        if (_waiting) //if waiting keep waiting until time is up
        {
            waitTimer += Time.deltaTime;
            if (waitTimer >= waitTime)
            {
                _waiting = false;
                SetDestination();
            }
        }

        } 
    }


