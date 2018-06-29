using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MushRoom : MonoBehaviour {

    private Rigidbody2D myRigid; // Rigidbody에 Friction 0인 physics material을 넣어준다. 벽에 부딪혔을시 안내려가는 문제를 없앤다.
    private float Speed = 2.0f;
    public LayerMask detectWhat;

    public Transform sightStart;
    public Transform sightEnd;

	// Use this for initialization
	void Start () {
        myRigid = GetComponent<Rigidbody2D>();

        Physics2D.IgnoreLayerCollision(9, 10); // 레이어간 충돌 무시
	}
	
	// Update is called once per frame
	void Update () {
        

        RaycastHit2D hit;
        hit = Physics2D.Linecast(sightStart.position, sightEnd.position, detectWhat);
        
        if(hit.collider != null)
        {
            Speed *= -1;
            transform.localScale = new Vector3(transform.localScale.x * -1.0f, 1);

        }

        myRigid.velocity = new Vector2(Speed, myRigid.velocity.y);


    }
    
    // 함수 도움말 만들기
    /// <summary>
    /// 1이면 오른쪽 -1이면 왼쪽
    /// </summary>
    public void ChangeDirection(int direction)
    {
        Speed = Speed * direction;
        transform.localScale = new Vector3(direction, 1);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(sightStart.position, sightEnd.position);
    }
}
