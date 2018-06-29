using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    //void OnCollisionEnter2D(Collision2D collision)
    //{
    //    if(collision.gameObject.tag == "Player")
    //    {
    //        collision.gameObject.GetComponent<Player>().coin++;
    //    }
    //}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            collision.gameObject.GetComponent<Player>().GetCoin();
            Destroy(gameObject);
        }
    }

    // A 사이트 : rigidbody2d, collider2D, OnTriggerEnter2D
    // B 사이트 : collider2D - isTrigger v
    // 충돌감지는 어디까지나 rigidbody2d가 하는 역할이다.
}
