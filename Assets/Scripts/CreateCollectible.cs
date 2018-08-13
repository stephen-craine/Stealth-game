﻿using System.Collections;
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
    
	void OnTriggerEnter (Collider other) {
        if (collected == false && other.CompareTag("Player"))
        {
            Instantiate(Prefab, Spawnpoint.position, Spawnpoint.rotation);
            collected = true;
        }
	}

    
	
	// Update is called once per frame
	void Update () {
		
	}
}
