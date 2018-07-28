using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    public float moveSpeed = 12;
    public float turnSpeed = 90;


    void Start()
    {
    }


    void Update()
    {
        var turnPlayer = Input.GetAxis("Horizontal") * Time.deltaTime * turnSpeed;
        var moveVertical = Input.GetAxis("Vertical") * Time.deltaTime * moveSpeed;
        transform.Translate(0, 0, moveVertical);
        transform.Rotate(0, turnPlayer, 0);


    }
}



