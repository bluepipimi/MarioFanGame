using UnityEngine;
using System.Collections;

public class Gumba : MonoBehaviour {

    public float velocity = 1f;

    public Transform sightStart;
    public Transform sightEnd;

    public LayerMask detectWhat;

    public bool colliding;
    private bool isDie;
    private bool isStop;
    private Rigidbody2D rigid;
    private CircleCollider2D cirCol;

    public Transform weakness;

    private Animator anim;

    private Vector3 startPos;
    private AudioSource myAudio;
    public AudioClip[] myAudioClip;
	// Use this for initialization
	void Start () {
        rigid = GetComponent<Rigidbody2D>();
        cirCol = GetComponent<CircleCollider2D>();
        anim = GetComponent<Animator>();
        myAudio = GetComponent<AudioSource>();
        startPos = transform.position;
        isDie = false;
	}

    // Update is called once per frame
    void Update() {

        if (!isDie)
        {
            rigid.velocity = new Vector2(velocity, rigid.velocity.y);
            colliding = Physics2D.Linecast(sightStart.position, sightEnd.position, detectWhat);

            if (colliding)
            {
                //print("Gumba");
                transform.localScale =
                    new Vector2(transform.localScale.x * -1, transform.localScale.y);
                velocity *= -1;
            }
        }

        if (isStop)
        {
            rigid.velocity = new Vector2(0, 0);
            rigid.gravityScale = 0;
            anim.speed = 0;
        }
        else
        {
            rigid.gravityScale = 1.0f;
            anim.speed = 1;
        }
            

        if (this.transform.position.y <= -100)
        {
            if (isDie)
                Destroy(gameObject);
            else
                Regen();
        }
        
	}

    public void SetStop(bool stop)
    {
        isStop = stop;
    }
    void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawLine(sightStart.position, sightEnd.position);
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if(col.gameObject.tag == "Player")
        {
            Player player = col.gameObject.GetComponent<Player>();
            if (player.bStar == true)
            {
                float dir = col.transform.position.x - transform.position.x;
                FireballDie(dir);
            }
            else
            {
                //float height = col.contacts[0].point.y - weakness.position.y;
                float height = player.foot.transform.position.y
                    - weakness.position.y;

                //print(height);
                if (height >= 0)
                {
                    StompDie();
                    col.rigidbody.velocity = new Vector2(0, 0);
                    col.rigidbody.AddForce(new Vector2(0, 400));
                }
            }
            
        }

        if (col.gameObject.tag == "fireball")
        {
            col.gameObject.GetComponent<CircleCollider2D>().enabled = false;
            float dir = col.transform.position.x - transform.position.x;
            //Debug.print("fireballDir : " + dir);
            FireballDie(dir);
        }
            
    }

    public void StompDie() // 밟혀 죽음
    {
        myAudio.clip = myAudioClip[0];
        myAudio.Play();
        anim.SetBool("stomped", true);
        Destroy(this.gameObject, 0.5f);
        isDie = true;
    }

    void FireballDie(float dir) // 불꽃 맞아 죽음
    {
        myAudio.clip = myAudioClip[1];
        myAudio.Play();

        
        if (!isDie)
        {
            transform.Rotate(0, 0, 180.0f);
        }
            

        float dieDir;
        if (dir > 0)
            dieDir = -1;
        else
            dieDir = 1;
        rigid.velocity = new Vector2(dieDir * 1.5f, 4);
       
        cirCol.enabled = false;
        isDie = true;
        rigid.gravityScale = 2;
        
    }

    public void blockDie()
    {
        FireballDie(-rigid.velocity.x);
    }

    void Regen()
    {
        this.rigid.velocity = new Vector2(velocity, 0);
        this.transform.position = startPos;
    }
}
