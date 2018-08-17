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
    public float checkTimer;
    public GameObject[] waypoints;
    Dictionary<GameObject, bool> testDict;
    public int testTrue;
    public bool testFinished;
    public int numberOfTests;
    public float totalTestTime;

    public enum State
    {
        NORMAL,
        SUSPICIOUS,
        WIN
                
    }
    public bool running;
    public State state;


    public void Start () {
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
        guardList = GameObject.FindGameObjectsWithTag("Guard"); //populate guardList at start- each guard can be referenced individually using name
        foreach (GameObject guard in guardList)
        {
            locDict.Add(guard, guard.GetComponent<AIPathfinding>().sectorName);
        }

        //state = State.NORMAL;
        //running = true;
        //StartCoroutine("FSM");
	}
	
    //IEnumerator FSM()
    //{
    //    while (running)
    //    {
    //        switch (state)
    //        {
    //            case State.NORMAL:
    //                Normal();
    //                break;
    //            case State.SUSPICIOUS:
    //                Suspicious();
    //                break;
    //            case State.WIN:
    //                Win();
    //                break;
    //        }
    //        yield return null;
    //    }
    //}

    //private void Normal()
    //{
    //    throw new NotImplementedException();
    //}

    //private void Suspicious()
    //{
    //    throw new NotImplementedException();
    //}

    //private void Win()
    //{
    //    throw new NotImplementedException();
    //}

    // Update is called once per frame
    public void Update()
    {
        checkTimer += Time.deltaTime * 1;
        //locDict updates@
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
