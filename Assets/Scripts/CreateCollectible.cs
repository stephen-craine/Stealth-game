using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateCollectible : MonoBehaviour {

    public Transform Spawnpoint;
    public GameObject Prefab;
    private bool collectedA;

    private void Start()
    {
        collectedA = false;
    }
    
	void OnTriggerEnter () {
        if (collectedA == false)
        {
            Instantiate(Prefab, Spawnpoint.position, Spawnpoint.rotation);
            collectedA = true;
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
