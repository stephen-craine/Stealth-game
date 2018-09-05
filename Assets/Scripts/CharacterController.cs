using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CharacterController : MonoBehaviour
{
<<<<<<< HEAD
    public float moveSpeed = 12f;
    public Text scoreText;
    public Text winText;
=======
    public float moveSpeed = 12;
>>>>>>> parent of 60fc7fc... Simple waypoint for AI pathing
    public Camera playerCamera;
    public NavMeshAgent playerAI;


    void Start()
    {
    }



    void Update()
    {
<<<<<<< HEAD

        //move relative to world space (so rotation doesn't affect movement)
        var x = moveSpeed * Input.GetAxis("Horizontal") * Time.deltaTime;
        var z = moveSpeed * Input.GetAxis("Vertical") * Time.deltaTime;
        transform.Translate(x, 0f, z, Space.World);

        //work out angle using atan 2
        float angle = Mathf.Atan2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")) * Mathf.Rad2Deg;

        // prevent cube snapping to angle if there is no movement
        if (Input.GetAxis("Horizontal") != 0f || Input.GetAxis("Vertical") != 0f)
=======
        if (Input.GetMouseButtonDown(0))
>>>>>>> parent of 60fc7fc... Simple waypoint for AI pathing
        {
            transform.rotation = Quaternion.Euler(0f, angle, 0f);
        }
<<<<<<< HEAD
    }
    //NavMesh controls if using player as AI agent for click to move).
    //if (Input.GetMouseButtonDown(0))
    //{
    //    Ray playerRay = playerCamera.ScreenPointToRay(Input.mousePosition);
    //    RaycastHit hit;
    //    if (Physics.Raycast(playerRay, out hit)) //Check clicked point is a possible position
    //    {
    //        //Moving player
    //        playerAI.SetDestination(hit.point);
    //    }
    //}

=======
>>>>>>> parent of 60fc7fc... Simple waypoint for AI pathing


    }
}



