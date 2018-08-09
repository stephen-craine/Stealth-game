using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class CharacterController : MonoBehaviour
{
    public float moveSpeed = 12f;
    public Text scoreText;
    public Text winText;
    public Camera playerCamera;
    public NavMeshAgent playerAI;
    private int score;


    void Start()
    {
        score = 0;
        setScore();
        winText.text = "";
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Key"))
        {
            other.gameObject.SetActive(false);
            score += 1;
            setScore();
        }    
    }

    void Update()
    {

        //move relative to world space (so rotation doesn't affect movement)
        var x = moveSpeed * Input.GetAxis("Horizontal") * Time.deltaTime;
        var z = moveSpeed * Input.GetAxis("Vertical") * Time.deltaTime;
        transform.Translate(x, 0f, z, Space.World);

        //work out angle using atan 2
        float angle = Mathf.Atan2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")) * Mathf.Rad2Deg;

        // prevent cube snapping to angle if there is no movement
        if (Input.GetAxis("Horizontal") != 0f || Input.GetAxis("Vertical") != 0f)
        {
            transform.rotation = Quaternion.Euler(0f, angle, 0f);
        }
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



    void setScore()
    {
        scoreText.text = "Score: " + score.ToString();
        if(score >= 1)
        {
            winText.text = "You did it!";
        }
    }
}



