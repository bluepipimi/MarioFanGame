using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour {

    public enum Item
    {
        Coin, Mushroom, Flower, Star,
    }

    public Item myItem;

    private Vector3 posA;
    private Vector3 posB;
    private Vector3 nextPos;
    private bool trigger;
    private bool bPunched;
    private bool bEmpty;  // 스태틱 필드는 모든 클래스가 곹유하는 필드이다.
    private int cycle;
    private int hitTimes; // 벽돌을 때린횟수
    public int MaxHitTImes; // 벽돌을 때릴 수 있는 최대 횟수
    private Animator myAnimator;
    private AudioSource myAudioSrc;
    public AudioClip[] myAudioClips;

    [SerializeField]
    private float speed;

    public GameObject coinEffect;
    public GameObject mushroom;
    public GameObject flower;
    public GameObject star;

    [SerializeField]
    private Transform endTransform;

    private BoxCollider2D[] myBoxes;

	// Use this for initialization
	void Start () {
       
        posA = this.transform.position;
        posB = endTransform.position;
        nextPos = posB;
        myAnimator = GetComponent<Animator>();
        myAudioSrc = GetComponent<AudioSource>();
        myBoxes = GetComponents<BoxCollider2D>(); // 박스콜라이거0 :트리거충돌 / 박스콜라이더1 : 일반충돌
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.H)) // 테스트 코드
        {
            trigger = true;
        }

        hit();

        if (myAnimator.GetBool("hit")){
            bEmpty = true;
        }
        
    }

    void hit()
    {
        if(trigger && cycle < 2) // 마리오 펀치를 맞으면 2번들어가는데 1번째는 올라가고 2번째는 내려간다.
        {
            transform.position =
           Vector3.MoveTowards(transform.position,
           nextPos, speed * Time.deltaTime);

            if (Vector3.Distance(transform.position, nextPos) <= 0.01)
            {
                ChangeDestination();
                cycle++;
            }
            myBoxes[0].enabled = false;
            bPunched = true;

        }
            
        if (cycle == 2)
        {
            hitTimes++;
            //print(hitTimes);
            trigger = false;
            cycle = 0;
            myBoxes[0].enabled = true;
            bPunched = false;
        }
            
        
    }

    void ChangeDestination()
    {
        nextPos = nextPos != posA ? posA : posB;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        
        if(collision.tag == "punch" && !bEmpty)
        {
            collision.gameObject.GetComponentInParent<Rigidbody2D>().velocity = new Vector2(0, 0); // 마리오가 박스에 부딪히면 상승속도를 0으로 만듬.

            bPunched = true;
            if (!trigger)
            {
                Player player = collision.gameObject.GetComponentInParent<Player>();
                switch (myItem)
                {
                    
                    case Item.Coin:
                        player.GetCoin();
                        GameObject effect = Instantiate(coinEffect, this.transform.position + Vector3.up, Quaternion.identity);
                        Destroy(effect, 1.0f);
                        myAudioSrc.clip = myAudioClips[0];
                        break;
                    case Item.Mushroom:
                        GameObject GO = Instantiate(mushroom, this.transform.position + Vector3.up, Quaternion.identity);
                        MushRoom ms = GO.GetComponent<MushRoom>();
                        if (player.facingRight)
                        {
                            ms.ChangeDirection(1);
                        }
                        else
                        {
                            ms.ChangeDirection(-1);
                        }
                        myAudioSrc.clip = myAudioClips[1];
                        break;
                    case Item.Flower:
                        Instantiate(flower, this.transform.position + Vector3.up, Quaternion.identity);
                        myAudioSrc.clip = myAudioClips[1];
                        break;
                    case Item.Star:
                        Instantiate(star, this.transform.position + Vector3.up, Quaternion.identity);
                        myAudioSrc.clip = myAudioClips[1];
                        break;
                }
                
                myAudioSrc.Play();
                
            }
                
            if(hitTimes >= MaxHitTImes)
                myAnimator.SetBool("hit", true);
            trigger = true;

            


        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if(bPunched)
        {
            if (collision.gameObject.tag == "deadly")
            {
                GameObject enemy = collision.gameObject;
                enemy.GetComponent<Gumba>().blockDie();
            }
        }
    }
    
}
