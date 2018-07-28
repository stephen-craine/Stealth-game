using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    // Update is called once per frame
    public Transform target;
    public float distance;
    void Update () {


        transform.position = new Vector3(target.position.x, target.position.y + 10, target.position.z - distance);

	}
}
