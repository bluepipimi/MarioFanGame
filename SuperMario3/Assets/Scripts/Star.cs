using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Star : MonoBehaviour {

    private Rigidbody2D myRigid;
    private BoxCollider2D myCol;
    public LayerMask Ground;
    public Transform posA;
    public Transform posB;
    private float speed;

    private bool bCollision;
	// Use this for initialization
	void Start () {
        myRigid = GetComponent<Rigidbody2D>();
        myCol = GetComponent<BoxCollider2D>();
        speed = 2.0f;
	}

    // Update is called once per frame
    void Update()
    {
        myRigid.velocity = new Vector2(speed, myRigid.velocity.y);

        if (Physics2D.Linecast(posA.position, posB.position, Ground))
        {
            print("sss");
            ChangeDirection();
        }
            

        

    }

    void ChangeDirection()
    {
        speed *= -1;
        print("ddd");
        transform.localScale = new Vector2(transform.localScale.x * -1, 1);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(bCollision == false && collision.gameObject.tag != "star")
        {
            myRigid.AddForce(new Vector3(0, 300.0f));
            bCollision = true;
        }
        
        switch(collision.gameObject.tag)
        {
            case "mushroom":
            case "flower":
            case "star":
            case "deadly":
                Physics2D.IgnoreCollision(myCol, collision.collider);
                break;
            
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        bCollision = false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(posA.position, posB.position);
    }
}
