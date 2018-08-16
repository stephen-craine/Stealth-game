using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Game Director will try and balance patrols equally between sectors

public class GameDirector : MonoBehaviour {

    //variables
    public GameObject[] guardList; 
    //public Dictionary<string, GameObject> guardMap;  //key: name, value: guard i.e. map the guards with their names
    public Dictionary<GameObject, string> locDict; //key: guard GameObject, value: sector location that they are colliding with (could be null if between sectors)
    

    public enum State
    {
        NORMAL,
        SUSPICIOUS,
        WIN
                
    }
    public bool running;
    public State state;


    public void Start () {
        locDict = new Dictionary<GameObject, string>();
        //guardMap = new Dictionary<string, GameObject>();
        guardList = GameObject.FindGameObjectsWithTag("Guard"); //populate guardList at start- each guard can be referenced individually using name
        foreach (GameObject guard in guardList)
        {
            locDict.Add(guard, guard.GetComponent<AIPathfinding>().sectorName);
        }

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

    private void Normal()
    {
        throw new NotImplementedException();
    }

    private void Suspicious()
    {
        throw new NotImplementedException();
    }

    private void Win()
    {
        throw new NotImplementedException();
    }

    // Update is called once per frame
    public void Update () {
        Debug.Log("Hello");
        if (locDict != null)
        {
            
            List<GameObject> keys = new List<GameObject>(locDict.Keys);
            foreach (GameObject guardKey in keys)
            {       
                
                locDict[guardKey] = guardKey.GetComponent<AIPathfinding>().sectorName;
            }

        }

	}
}
