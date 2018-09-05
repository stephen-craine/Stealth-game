using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

//Game Director will try and balance patrols equally between sectors

public class GameDirector : MonoBehaviour {

    //variables
    public GameObject[] guardList;
    //public Dictionary<string, GameObject> guardMap;  //key: name, value: guard i.e. map the guards with their names
    public Dictionary<GameObject, string> locDict; //key: guard GameObject, value: sector location that they are colliding with (could be null if between sectors)
    public Dictionary<GameObject, ConnectedWaypoint> initialDict = new Dictionary<GameObject, ConnectedWaypoint>();
   
    public float checkTimer;
    public GameObject[] waypoints;
    Dictionary<GameObject, bool> testDict;
    public int testTrue;
    public bool testFinished;
    public int numberOfTests;
    public float totalTestTime;
    public bool startSuspicious;
    public GameObject[] sectorList;
    public List<GameObject> _sectors;
    public bool enterNormal;
    public bool paused;
    public List<GameObject> randomInitialList;
    public GameObject chasingGuard;
    public bool resetChaseTimer;
    public float chaseTimer;



    public enum State
    {
        NORMAL,
        SUSPICIOUS,
        WIN
                
    }
    public bool running;
    public State state;


    public void Awake()
    {
        paused = true;
        GetInitial();
    }

    public void Start () {
        
        _sectors = new List<GameObject>();
        guardList =  GameObject.FindGameObjectsWithTag("Guard");
        string[] sectorsWithWPS = new string[] { "NW", "NE", "SW", "SE" };
        sectorList = GameObject.FindGameObjectsWithTag("Sector");

        foreach(GameObject i in sectorList)
        {
            foreach(string j in sectorsWithWPS)
            {
                if (string.Equals(i.name, j))
                {
                    _sectors.Add(i); //list of sectors that contain waypoints
                }
            }
            
        }

        resetChaseTimer = false;
        chaseTimer = 0;
         
        //for testing
        totalTestTime = 0;
        numberOfTests = 1;
        testFinished = false;
        checkTimer = 0;
        waypoints = GameObject.FindGameObjectsWithTag("Waypoint");

        testDict = new Dictionary<GameObject, bool>(); 
        foreach (GameObject _waypoint in waypoints)
        {
            testDict.Add(_waypoint, false);
        }

        //for pathfinding
        locDict = new Dictionary<GameObject, string>();
        //guardMap = new Dictionary<string, GameObject>();
         //populate guardList at start- each guard can be referenced individually using name
        foreach (GameObject guard in guardList)
        {
            locDict.Add(guard, guard.GetComponent<AIPathfinding>().sectorName);
        }
        enterNormal = true;
        state = State.NORMAL;
        running = true;
        StartCoroutine("FSM");
    }

    IEnumerator FSM()
    {
        while (running)
        {
            switch (state)
            {
                case State.NORMAL:
                    Normal();
                    break;
                case State.SUSPICIOUS:
                    Suspicious();
                    break;
                case State.WIN:
                    Win();
                    break;
            }
            yield return null;
        }
    }

    public void GetInitial()
    {
        if (guardList.Length == 4)
        {
            int i = 0;

            initialDict = new Dictionary<GameObject, ConnectedWaypoint>();
            if (guardList != null && _sectors != null)
            {
                while (i < guardList.Length)
                {

                    randomInitialList = new List<GameObject>();
                    randomInitialList = _sectors[i].GetComponent<SectorScript>().wpsInSector;
                    //add patrolling sectors 1 per guard i.e. give them an initial waypoint in each sector as waypoints are connected by sector and they start new patrol route
                    int random = UnityEngine.Random.Range(0, _sectors.Count);
                    initialDict.Add(guardList[i], randomInitialList[random].GetComponent<ConnectedWaypoint>());
                    i++;

                }
                paused = false;
            }
        }
        
    }
        public void Normal()
    {
        if (enterNormal == true)
        {
            GetInitial();
            chaseTimer = 0;
            resetChaseTimer = false;
            enterNormal = false;
        }
        }
       
    public void Suspicious()
    {
        while (startSuspicious == true) // enter state code
        {
            List<GameObject> suspiciousGuards = new List<GameObject>(); //list of guards not in active chase to move to guard the sector
            foreach (GameObject guard in guardList)
            {
                if (guard.GetComponent<AIPathfinding>().state == AIPathfinding.State.CHASE)
                {
                    GameObject chasingGuard = guard;
                }
                else
                {
                    suspiciousGuards.Add(guard);
                }
            }

            //if chasing move to a waypoint at random from sector
            //if chase time == chaseTotalTime then move guards back to patrol state and move back to normal director state

            chasingGuard.GetComponent<AIPathfinding>().CheckMySector();
            string currentSector = chasingGuard.GetComponent<AIPathfinding>().sectorName;


            startSuspicious = false;
        }
    }
    
    private void Win()
    {
        throw new NotImplementedException();
    }



    // Update is called once per frame
    public void Update()

    {   ///////////// Handling change from Suspicious to Normal
        foreach(GameObject guard in guardList)
        {
            if (guard.GetComponent<AIPathfinding>().SpotPlayer())
            {
                resetChaseTimer = true;
            }
        }

        if(state == State.SUSPICIOUS && resetChaseTimer == true)
        {
            chaseTimer = 0;
            resetChaseTimer = false;
        }

        if(state == State.SUSPICIOUS && resetChaseTimer == false)
        {
            chaseTimer += Time.deltaTime * 1;
        }

        if(chaseTimer >= 5)
        {
            state = State.NORMAL;
            foreach(GameObject guard in guardList)
            {
                guard.GetComponent<AIPathfinding>().state = AIPathfinding.State.PATROL;
            }
        }
        //////////////


        foreach(GameObject guard in guardList)
        {
            if(guard.GetComponent<AIPathfinding>().state == AIPathfinding.State.CHASE)
            {
                state = State.SUSPICIOUS;
            }
        }

        checkTimer += Time.deltaTime * 1;
        //locDict updates
        if (locDict != null)
        {

            List<GameObject> keys = new List<GameObject>(locDict.Keys);
            foreach (GameObject guardKey in keys)
            {

                locDict[guardKey] = guardKey.GetComponent<AIPathfinding>().sectorName;
            }

        }
        //testDict updates:
        testTrue = 0;
        if (testDict != null && testFinished == false)
        {
            List<GameObject> wKeys = new List<GameObject>(testDict.Keys);
            foreach (GameObject wKey in wKeys)
            {
                testDict[wKey] = wKey.GetComponent<ConnectedWaypoint>().check;

                if (testDict[wKey] == true)
                {
                    testTrue += 1;
                }

            }

            if (testTrue == waypoints.Length && numberOfTests <= 100)
            {
                //If all the waypoints are checked at least once, end timer
                string text = numberOfTests.ToString() + ":      " + checkTimer.ToString();
                WriteText(text);
                testFinished = true;
            }
        }

            if(testFinished == true && numberOfTests <= 100) //then reset all dictionary waypoint values and actual waypoint checks to false to repeat the test
            {
                numberOfTests += 1;
                List<GameObject> dKeys = new List<GameObject>(testDict.Keys);
                foreach(GameObject dKey in dKeys)
                {
                testDict[dKey] = false;
                dKey.GetComponent<ConnectedWaypoint>().check = false;
                }
            testFinished = false;
            totalTestTime += checkTimer;
            checkTimer = 0;
            }

            if(numberOfTests == 101)
        {
            WriteText("Total time for 100 tests:   " + totalTestTime.ToString());
            numberOfTests += 1; //stop testing
            WriteText("Average time for complete patrol:   " + (totalTestTime / 100).ToString());
        }


        
    }

    public void WriteText(string textA)
    {
        string pathToWrite = "Assets/Testing/test.txt";
        StreamWriter writer = new StreamWriter(pathToWrite, true);
        writer.WriteLine(textA);
        writer.Close();
        testFinished = true;
 
    }
	
}
