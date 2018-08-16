using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectedWaypoint : Waypoint { //subclass of waypoint to find nearby waypoints to make a sensible random path for the AI.

    [SerializeField] protected float _connectivityRadius = 40F;

    [SerializeField] public bool beingVisited; //to stop two AI picking the same waypoint as next destination
    [SerializeField] public int weightWaypoint; //variables for optimising patrolling of multiple AI

    //List<ConnectedWaypoint> _connections;
    public Dictionary<ConnectedWaypoint, int> _connections = new Dictionary<ConnectedWaypoint, int>();
    public List<ConnectedWaypoint> _keys;
    public List<ConnectedWaypoint> _weightedConnections = new List<ConnectedWaypoint>();
    public string _sector;

    public void Start()
    {
        



        beingVisited = false;
        weightWaypoint = 0;
        GameObject[] allWaypoints = GameObject.FindGameObjectsWithTag("Waypoint");
        //_connections = new List<ConnectedWaypoint>();

        for (int i = 0; i < allWaypoints.Length; i++)
        {
            ConnectedWaypoint nextWaypoint = allWaypoints[i].GetComponent<ConnectedWaypoint>();
            if(nextWaypoint != null && nextWaypoint != this)
            {
                if(Vector3.Distance(this.transform.position, nextWaypoint.transform.position) <= _connectivityRadius) //if near any other waypoints 
                {
                    _connections.Add(nextWaypoint, 0); //add to dictionary with weight of 0.
                }
            }

        }
        List<ConnectedWaypoint> keys = new List<ConnectedWaypoint>(_connections.Keys);
        foreach (ConnectedWaypoint item in keys) {
            _keys.Add(item);
        }
    }

    public void CheckSector() 
    {
        Collider[] hitColliders = Physics.OverlapSphere(this.transform.position, 1);
        int i = 0;
        while(i< hitColliders.Length)
        {
            Collider current = hitColliders[i];
            if(current.tag == "Sector")
            {
                _sector = current.gameObject.name;
            }
            i++;
        }
        
    }


    public void FixedUpdate()
    {

        foreach(ConnectedWaypoint c in _keys)
        {
            _connections[c] = c.GetComponent<ConnectedWaypoint>().weightWaypoint; //update current weight in dictionary
        }




    }
    //public override void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.yellow;
    //    Gizmos.DrawWireSphere(transform.position, debugDrawRadius);

    //    Gizmos.color = Color.green;
    //    Gizmos.DrawWireSphere(transform.position, _connectivityRadius);
    //}


    public void Update()
    {
        CheckSector();
        //form list of waypoints with lowest weight do choose randomly from EXCLUDING if they are already 'being visited'
        
        List<int> _values = new List<int>(_connections.Values);
        _values.Sort();
        int minValue = _values[0];

        foreach (ConnectedWaypoint d in _keys)
        {
            if (_connections[d] == minValue)
            {
                if (d.GetComponent<ConnectedWaypoint>().beingVisited == false)
                {
                    if (!_weightedConnections.Contains(d))
                    {
                        _weightedConnections.Add(d); //if it has lowest weight, is not being visited, and is not already in the list then add it.
                    }
                }
            }
            if (_connections[d] > minValue || d.GetComponent<ConnectedWaypoint>().beingVisited) //if a waypoint is being visited or has a higher than minimum weight, delete it from possible connections
            {
                if (_weightedConnections.Contains(d))
                {
                    _weightedConnections.Remove(d);
                }
            }
        }
        /////////////////////////////////////////////////////////////////
    }


    public ConnectedWaypoint NextWaypoint(ConnectedWaypoint previousWaypoint)
    {

        if(_connections.Count == 0)
        {
            Debug.LogError("No nearby waypoints found!");
            return null;
        } else if(_connections.Count == 1 && _connections.ContainsKey(previousWaypoint)) { 
                return previousWaypoint;
        } else {



            ConnectedWaypoint nextWaypoint = previousWaypoint; //if cannot find one to visit, return to previous waypoint - only time two AI will visit same spot.
            int nextWaypointIndex = 0;
                if (_weightedConnections.Count > 0)
                {
                    nextWaypointIndex = UnityEngine.Random.Range(0, _weightedConnections.Count);
                    nextWaypoint = _weightedConnections[nextWaypointIndex];
                } 
                    
            return nextWaypoint;
        }

        }

    public void WeightWaypoint()
    {
        this.beingVisited = true;
        this.weightWaypoint += 1;
    }
}

