using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameDirector : MonoBehaviour {

    //variables



    public enum State
    {
        NORMAL,
        SUSPICIOUS,
        WIN
                
    }
    public bool running;
    public State state;


    void Start () {






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
    void Update () {
		
	}
}
