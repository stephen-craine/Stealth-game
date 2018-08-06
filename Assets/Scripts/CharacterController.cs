using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CharacterController : MonoBehaviour
{
    public float moveSpeed = 12;
    public Camera playerCamera;
    public NavMeshAgent playerAI;


    void Start()
    {
    }



    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray playerRay = playerCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(playerRay, out hit)) //Check clicked point is a possible position
            {
                //Moving player
                playerAI.SetDestination(hit.point);
            }
        }


    }
}



