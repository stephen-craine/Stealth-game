using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateCollectible : MonoBehaviour {

    public Transform Spawnpoint;
    public GameObject Prefab;
    [SerializeField] public bool collected;

    private void Start()
    {
        collected = false;
    }
    
	void OnTriggerEnter () {
        if (collected == false)
        {
            Instantiate(Prefab, Spawnpoint.position, Spawnpoint.rotation);
            collected = true;
        }
	}

    
	
	// Update is called once per frame
	void Update () {
		
	}
}
