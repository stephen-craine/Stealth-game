using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIPathfinding : MonoBehaviour {

    [SerializeField] //allows viewing/editing of AIDestination in the Unity editor
    Transform AIDestination;

    NavMeshAgent navMeshAgent;

	void Start () {

        navMeshAgent = this.GetComponent<NavMeshAgent>();

        if(navMeshAgent == null)
        {
            Debug.LogError("Nav mesh agent component not attached to: " + gameObject.name);
        }
        else
        {
            SetDestination();
        }

	}

    private void SetDestination()
    {
        if(AIDestination != null)
        {
            Vector3 targetVector = AIDestination.transform.position;
            navMeshAgent.SetDestination(targetVector);
        }
    }

}
