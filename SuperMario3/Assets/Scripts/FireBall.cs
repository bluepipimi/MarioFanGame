using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBall : MonoBehaviour {

    public GameObject smokeEffect;
    private Rigidbody2D rb;
    private Vector2 velocity;
    private bool bStop;

	// Use this for initialization
	void Start () {
        Destroy(gameObject, 3.0f);
        rb = GetComponent<Rigidbody2D>();
        velocity = rb.velocity;

	}
	
	// Update is called once per frame
	void Update () {

        if (!bStop)
        {
            if (rb.velocity.y < velocity.y) // 중력의 영향을 안받는다.
                rb.velocity = velocity;
            rb.gravityScale = 3;
        }
        else
        {
            rb.gravityScale = 0;
            rb.velocity = new Vector3(0, 0);
        }
        
        
    }

    public void SetStop(bool stop)
    {
        bStop = stop;
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        rb.velocity = new Vector2(velocity.x, -velocity.y); // 바닥에 닿았을 때 튕겨나간다.

        if(col.collider.tag == "deadly")
        {   
            Explode(false);
        }

        float xn = col.contacts[0].normal.x;
        if (col.collider.tag != "deadly" && Mathf.Abs(xn) > 0.7f) // Collision Detection : Continuous
        {
            Explode(true);
        }
    }

    void Explode(bool soundPlay)
    {
        GetComponent<CircleCollider2D>().enabled = false;
        Destroy(this.gameObject);
        
        GameObject smoke = (GameObject)Instantiate(smokeEffect, this.transform.position, Quaternion.identity);
        if(soundPlay)
            smoke.GetComponent<AudioSource>().Play();
        Destroy(smoke, 0.3f); // 애니메이션이 끝난 후에 없애줘야 함으로 0.3초로 했다.
        
    }

}
