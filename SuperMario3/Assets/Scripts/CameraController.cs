using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    public float MinX;  // -3.62
    public float MaxX;  // -0.54

    public Transform target;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        transform.position = new Vector3(
            Mathf.Clamp(target.position.x, MinX, MaxX),
            transform.position.y,
            transform.position.z);
	}
}
