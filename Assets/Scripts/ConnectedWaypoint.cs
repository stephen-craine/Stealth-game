using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectedWaypoint : Waypoint { //subclass of waypoint to find nearby waypoints to make a sensible random path for the AI.

    [SerializeField] protected float _connectivityRadius = 40F;

    [SerializeField] public bool beingVisited; //to stop two AI picking the same waypoint as next destination
    [SerializeField] public int weightWaypoint; //variables for optimising patrolling of multiple AI

    List<ConnectedWaypoint> _connections;

    public void Start()
    {
        beingVisited = false;
        weightWaypoint = 0;
        GameObject[] allWaypoints = GameObject.FindGameObjectsWithTag("Waypoint");
        _connections = new List<ConnectedWaypoint>();

        for (int i = 0; i < allWaypoints.Length; i++)
        {
            ConnectedWaypoint nextWaypoint = allWaypoints[i].GetComponent<ConnectedWaypoint>();
            if(nextWaypoint != null && nextWaypoint != this)
            {
                if(Vector3.Distance(this.transform.position, nextWaypoint.transform.position) <= _connectivityRadius)
                {
                    _connections.Add(nextWaypoint); //add to list of nearby connected waypoints.
                }
            }

        }

    }
    //public override void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.yellow;
    //    Gizmos.DrawWireSphere(transform.position, debugDrawRadius);

    //    Gizmos.color = Color.green;
    //    Gizmos.DrawWireSphere(transform.position, _connectivityRadius);
    //}

    public ConnectedWaypoint NextWaypoint(ConnectedWaypoint previousWaypoint)
    {

        if(_connections.Count == 0)
        {
            Debug.LogError("No nearby waypoints found!");
            return null;
        } else if(_connections.Count == 1 && _connections.Contains(previousWaypoint)) { 
                return previousWaypoint;
        } else {
            ConnectedWaypoint nextWaypoint;
            int nextWaypointIndex = 0;

            do
            {
                nextWaypointIndex = UnityEngine.Random.Range(0, _connections.Count);
                nextWaypoint = _connections[nextWaypointIndex];

            } while (nextWaypoint == previousWaypoint); //if the previous waypoint is selected, loop again until the other is found!

            return nextWaypoint;
        }

        }

    public void WeightWaypoint()
    {
        this.beingVisited = true;
        this.weightWaypoint += 1;
    }
}

