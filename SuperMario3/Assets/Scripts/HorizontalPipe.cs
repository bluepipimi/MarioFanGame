using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HorizontalPipe : MonoBehaviour
{
    public enum PipeState
    {
        Idle,
        PutPlayer,
        TakePlayerOut,
    }

    public string pipeName;
    public GameObject PipeToGo;
    public Transform PositionA;
    public Transform PositionB;
    public GameObject player;
    private AudioSource myAudio;
    public int playTimes;
    public PipeState myState;
    public bool bPlayerGone;
    public Vector2 SceneChangePosition; // 씬전환 했을 때 캐릭터 위치 지정
    public bool bLoadNewScene;
    // public이기 때문에 인스펙터 창에 이변수가 보일 것이다. 인스펙터창에서 이 변수 부분에 체크하면 파이프에 들어갈 때 씬이 전환된다. 
    public string LoadLevelName;
    // 새로 로드할 씬의 이름을 적는다.

    // Use this for initialization
    void Start()
    {
        myAudio = GetComponent<AudioSource>();
        myState = PipeState.Idle;
        playTimes = 0;
        bPlayerGone = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (player && player.GetComponent<Player>().bGoRight && myState == PipeState.Idle)
        {
            myState = PipeState.PutPlayer;
        }

        if (player && myState == PipeState.PutPlayer)
        {
            EnterInPipe(player);
        }


        if (player && player.GetComponent<Player>().bStopInput && myState == PipeState.Idle && !bPlayerGone)
        {
            myState = PipeState.TakePlayerOut;
        }

        if (player && myState == PipeState.TakePlayerOut)
        {
            ExitOnPipe(player);
        }

        if (bPlayerGone)
        {
            player = null;
            bPlayerGone = false;
        }

    }

    void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            player = collision.gameObject;
            // Debug.print("playerInPipe");
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player" && myState == PipeState.Idle)
        {
            player = null;
        }
    }

    void EnterInPipe(GameObject player)
    {
        if (!myAudio.isPlaying && playTimes == 0)
        {
            myAudio.Play();
            playTimes = 1;
            
        }
        player.transform.position = new Vector2(player.transform.position.x, this.transform.position.y - 0.75f);
        player.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
        player.GetComponent<Player>().bStopInput = true;
        
        player.transform.position += Vector3.right * Time.deltaTime;
        player.GetComponent<CapsuleCollider2D>().isTrigger = true;
        player.GetComponentInChildren<CircleCollider2D>().isTrigger = true;
        player.GetComponent<Rigidbody2D>().gravityScale = 0.0f;
        player.GetComponent<SpriteRenderer>().sortingOrder = -1;

        if (player && Mathf.Abs(player.transform.position.x - PositionB.position.x) < 0.1f && myState == PipeState.PutPlayer)
        {
            if (bLoadNewScene)
            {
                GameManager.SetPlayerPosition(SceneChangePosition); 
                GameManager.SetEnterPipe(pipeName);
                SceneManager.LoadScene(LoadLevelName);
            }
                


            if (PipeToGo)
            {
                player.transform.position = new Vector2(PipeToGo.transform.position.x, PipeToGo.transform.position.y);
                myState = PipeState.Idle;
                playTimes = 0;
                bPlayerGone = true;
            }
        }
    }

    void ExitOnPipe(GameObject player)
    {
        if (!myAudio.isPlaying && playTimes == 0)
        {
            myAudio.Play();
            playTimes++;
        }

        if (player && Mathf.Abs(player.transform.position.x - PositionA.position.x) > 0.1f)
        {
            player.transform.position -= Vector3.right * Time.deltaTime;
        }
        else
        {
            myState = PipeState.Idle;
            player.GetComponent<Player>().bStopInput = false;
            player.GetComponent<CapsuleCollider2D>().isTrigger = false;
            player.GetComponent<Rigidbody2D>().gravityScale = 1.5f;
            player.GetComponent<SpriteRenderer>().sortingOrder = 0;
            playTimes = 0;
        }

    }
}
