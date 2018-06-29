using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestRaycast : MonoBehaviour {

    public float floatHeight;
    public float liftForce;
    public float damping;
    public Rigidbody2D rb2D;
    RaycastHit2D raycasthit;
    RaycastHit2D boxcasthit;
    // Use this for initialization
    void Start () {
        rb2D = GetComponent<Rigidbody2D>();	
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        raycasthit = Physics2D.Raycast(transform.position, -Vector2.up); // 현재 위치에서 아래방향으로 광선을 쏜다
        if(raycasthit.collider != null)
        {
            // 광선에 부딪힌 충돌체가 있다면 들어온다.
            //print("hit");
        }

        //hit = Physics2D.BoxCast(transform.position, new Vector2(1, 1), 0.0f, Vector3.up); // 현재 위치에서 위방향으로 네모광선을 쏜다.
        
        if(boxcasthit.collider != null)
        {
            
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, new Vector3(transform.position.x, transform.position.y - raycasthit.distance));
        
        //Gizmos.DrawWireCube(transform.position + Vector3.up, new Vector3(1, 1, 0));
    }
}
