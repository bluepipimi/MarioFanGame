using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gole : MonoBehaviour {

    GameObject playerGO;
    public Transform endPosition;   // 깃대봉 제일 하단
    public Transform doorPosition;  // 성문 입구
    public GameManager GM;
    bool eventTrigger;
    bool eventTrigger2;
    bool bAudioPlay;
    AudioSource myAudio;

	// Use this for initialization
	void Start () {
        myAudio = GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update () {

        if(playerGO)
        {
            Rigidbody2D rg = playerGO.GetComponent<Rigidbody2D>();

            

            if(playerGO.transform.position.y > endPosition.position.y) // 깃대봉 하단 고정블록 위에 플레이어 위치시
            {
                if (eventTrigger == false) // 봉내려오기
                {
                    playerGO.transform.position = new Vector3(endPosition.position.x - 0.4f, playerGO.transform.position.y);
                    rg.velocity = new Vector2(0, -3.0f);
                }
                    

                if (bAudioPlay == false && myAudio.isPlaying == false)
                {
                    myAudio.Play(); // 봉내려오는 소리
                    bAudioPlay = true;
                }
                    
            }
            else // 깃대봉 고정블록에 플레이어가 도착했을시
            {
                if (eventTrigger == false) // 깃대봉 반대쪽으로 넘어가기
                {
                    playerGO.transform.position = new Vector3(endPosition.position.x + 0.4f, playerGO.transform.position.y);
                    playerGO.transform.localScale = new Vector2(-1.0f, 1.0f);
                    playerGO.GetComponent<Animator>().speed = 0;
                    Invoke("EventTrigger", 1.0f);  // 1초뒤에 EventTrigger 함수 실행
                }
            }

            if(eventTrigger == true)
            {
                if(playerGO.transform.position.x < doorPosition.position.x) // 문보다 플레이어가 왼쪽에 있으면
                {
                    if (eventTrigger2 == false)
                        playerGO.transform.position += Vector3.right * Time.deltaTime * 2.0f;
                    CancelInvoke();
                    playerGO.GetComponent<Player>().bClear = true; // 클리어 BGM이 나온다.
                }
                else // 플레이어가 문에 도착함
                {
                    if(eventTrigger2 == false)
                    {
                        playerGO.GetComponent<Rigidbody2D>().velocity = new Vector3(0, 0);
                        playerGO.GetComponent<Animator>().SetTrigger("tCastle");
                        playerGO.GetComponent<Player>().bFlagPollTouch = false;
                        eventTrigger2 = true;
                        playerGO = null;
                    }
                    
                }
                


            }
        }
	}

    void EventTrigger()
    {
        playerGO.transform.localScale = new Vector2(1.0f, 1.0f);
        eventTrigger = true;
        playerGO.GetComponent<Animator>().SetTrigger("tGoal2"); // tGoal 트리거 두번째 사용
        print("b");
        playerGO.GetComponent<Animator>().speed = 1.0f;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            print("a");
            GM.StopBGM();
            playerGO = collision.gameObject;
            playerGO.GetComponent<Animator>().SetTrigger("tGoal");  // tGoal 트리거 첫번째 사용
            playerGO.GetComponent<Animator>().speed = 1.0f;
            playerGO.GetComponent<Player>().bStopInput = true;
            playerGO.GetComponent<Player>().bFlagPollTouch = true;
        }
    }
}
