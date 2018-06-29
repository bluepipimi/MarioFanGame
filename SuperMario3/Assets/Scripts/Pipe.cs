using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Pipe : MonoBehaviour
{
    public enum PipeState
    {
        Idle,
        PutPlayer,
        TakePlayerOut,
    }

    public GameObject PipeToGo;
    public Transform PositionA;
    public Transform PositionB;
    public GameObject player;
    private AudioSource myAudio;
    public int playTimes;
    public PipeState myState;
    public bool bPlayerGone;
    public bool bLoadNewScene;
    public string LoadLevelName;

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
        if(player && player.GetComponent<Player>().bCrouch && myState == PipeState.Idle)
        {
            myState = PipeState.PutPlayer;
        }

        if (player && myState == PipeState.PutPlayer)
        {
            EnterInPipe(player);
        }
            

        if(player && player.GetComponent<Player>().bStopInput && myState == PipeState.Idle && !bPlayerGone)
        {
            myState = PipeState.TakePlayerOut;
        }

        if(player && myState == PipeState.TakePlayerOut)
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
        if(collision.tag == "Player" && myState == PipeState.Idle)
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

        player.GetComponent<Player>().bCrouch = false;
        player.GetComponent<Player>().bStopInput = true;
        player.transform.position -= Vector3.up * Time.deltaTime;
        player.GetComponent<CapsuleCollider2D>().isTrigger = true;
        player.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
        player.GetComponent<Rigidbody2D>().gravityScale = 0.0f;
        player.GetComponent<SpriteRenderer>().sortingOrder = -1;

        if (player && Mathf.Abs(player.transform.position.y - PositionB.position.y) < 0.1f && myState == PipeState.PutPlayer)
        {
            if (bLoadNewScene)
                SceneManager.LoadScene(LoadLevelName);
            if(PipeToGo)
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
        if (player && Mathf.Abs(player.transform.position.y - PositionA.position.y) > 0.1f)
        {
            player.transform.position += Vector3.up * Time.deltaTime;
            player.GetComponentInChildren<CircleCollider2D>().isTrigger = true;
            player.GetComponent<Player>().bInThePipe = true;
        }
        else
        {
            myState = PipeState.Idle;
            player.GetComponent<Player>().bInThePipe = false;
            player.GetComponent<Player>().bStopInput = false;
            player.GetComponent<CapsuleCollider2D>().isTrigger = false;
            player.GetComponent<Rigidbody2D>().gravityScale = 3.0f;
            player.GetComponent<SpriteRenderer>().sortingOrder = 0;
            playTimes = 0;
        }

    }
}
