using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateCollectible : MonoBehaviour {

    public Transform Spawnpoint;
    public GameObject Prefab;

	void OnTriggerEnter () {
        Instantiate(Prefab, Spawnpoint.position, Spawnpoint.rotation);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
