using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Debug : MonoBehaviour {

    public Text txt2;
    public Text txt3;

    public GameObject player;
    public GameObject gumba;
    public GameObject pipe;

	// Use this for initialization
	void Start () {
        StartCoroutine(Debuging());
	}
	
	// Update is called once per frame
	void Update () {
        
    }

    IEnumerator Debuging()
    {
        while (true)
        {
            Debug.print("PipeState : " + pipe.GetComponent<Pipe>().myState);
            txt2.text = "PipeState : " + pipe.GetComponent<Pipe>().myState;
            yield return null;
        }
        
        
        
        //while (true) {
        //    if (gumba == null)
        //        break; 
        //    txt2.text = "Player " + Math.Round(player.transform.position.y,2).ToString();
        //    txt3.text = "Gumba " + Math.Round(gumba.transform.position.y,2).ToString();
        //    yield return null;
        //}
        
    }
}
