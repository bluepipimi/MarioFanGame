using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Brick : MonoBehaviour {

    public GameObject[] pieceOfBricks;

    public float breakPower;

    private BoxCollider2D[] myBoxColliders;
    private SpriteRenderer mySpriteRenderer;
    private AudioSource myAudio;
    public AudioClip[] myClips;
    private bool bPunched;

    private Vector3 posA;
    private Vector3 posB;
    private Vector3 nextPos;
    private bool trigger;
    public Transform endTransform;
    public float speed;

	// Use this for initialization
	void Start () {
        myBoxColliders = GetComponents<BoxCollider2D>(); // 0 col 1 trigger
        mySpriteRenderer = GetComponent<SpriteRenderer>();
        myAudio = GetComponent<AudioSource>();

        posA = transform.position;
        posB = endTransform.position; // 월드 좌표가 들어간다.
        nextPos = posB;
    }
	
	// Update is called once per frame
	void Update () {
        
	}

    private void OnTriggerEnter2D(Collider2D collision)
    {
       
        if(collision.tag == "punch" && trigger == false)
        {
            trigger = true; // 버그막기
            GameObject go = collision.gameObject;
            go.GetComponentInParent<Rigidbody2D>().velocity = new Vector2(go.GetComponentInParent<Rigidbody2D>().velocity.x, 0);
            if(Player.marioForm != Player.Formation.MARIO)
            {
                breakBrick();
            }
            else
            {
                StartCoroutine("shakeBrick");
                
            }
            

            
        }
    }

    IEnumerator shakeBrick()
    {
        yield return new WaitForSeconds(0.05f);
        myAudio.clip = myClips[1];
        myAudio.Play();
        for(int i =0; i<2;)
        { 
            transform.position = Vector2.Lerp(transform.position, nextPos, speed * Time.deltaTime);
            
            if (Vector2.Distance(transform.position, nextPos) <= 0.01f)
            {
                nextPos = nextPos == posA ? posB : posA;
                i++;
            }
           
            bPunched = true;
            yield return null;
        }
        
        bPunched = false;
        trigger = false;
        yield return null;
    }

    private void breakBrick()
    {
        bPunched = true;
        mySpriteRenderer.enabled = false;
        float offsetX;
        float offsetY;

        for (int i = 0; i < pieceOfBricks.Length; i++)
        {
            if (i % 2 == 0)
                offsetX = -0.1f;
            else
                offsetX = 0.1f;
            if (i < 2)
                offsetY = 0.1f;
            else
                offsetY = -0.1f;

            pieceOfBricks[i].transform.position = this.transform.position + new Vector3(offsetX, offsetY, 0);

            GameObject piece = Instantiate(pieceOfBricks[i]);
            myAudio.clip = myClips[0];
            myAudio.Play();

            Destroy(piece, 2);
            Destroy(gameObject, 2); // 사운드 효과 때문에 2초뒤에 없앰
            if (i == 0)
            {
                piece.GetComponent<Rigidbody2D>().AddForce(new Vector2(-breakPower - 20.0f, breakPower + 50.0f));
            }
            else if (i == 1)
            {
                piece.GetComponent<Rigidbody2D>().AddForce(new Vector2(breakPower + 20.0f, breakPower + 50.0f));
            }
            else if (i == 2)
            {
                piece.GetComponent<Rigidbody2D>().AddForce(new Vector2(-breakPower, breakPower));
            }
            else
            {
                piece.GetComponent<Rigidbody2D>().AddForce(new Vector2(breakPower, breakPower));
            }

            myBoxColliders[0].enabled = false;
            myBoxColliders[1].enabled = false;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // 벽돌과 플레이거가 한번 충돌후에 다시 충돌하지 않게 함
        if(collision.tag == "punch")
        {
            //myBoxColliders[0].enabled = false;
            //myBoxColliders[1].enabled = false;
        }
    }

    // Rigidbody는 충돌하는 두 물체 중에 하나만 붙으면 된다.
    private void OnCollisionStay2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "deadly")
        {
            if(bPunched)
            {
                GameObject enemy = collision.gameObject;

                enemy.GetComponent<Gumba>().blockDie();
            }
            
        }

        //if(collision.gameObject.tag == "Player")
        //{
        //    print("Player");
        //}
    }
}
