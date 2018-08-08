using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class CharacterController : MonoBehaviour
{
    public float moveSpeed = 12;
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
        //NavMesh controls
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

    void setScore()
    {
        scoreText.text = "Score: " + score.ToString();
        if(score >= 1)
        {
            winText.text = "You did it!";
        }
    }
}



