using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    static private Vector2 playerPosition;
    static private string EnterPipe;
    static private int Coin = 0;
    static float StartTime = 300.0f;
    static Player.Formation mformation; // 씬이 전환되어도 변신정보가 유지된다.
    static GameObject playerGO;
    public AudioClip[] audioClips;
    private string SceneName;
    AudioSource myAudio;
    int playTimes = 0;
    private bool bStop;
    private bool bPause;
    private float currentTime;
    private float pastTime;

    // Use this for initialization
    void Start() {
        playerGO = GameObject.Find("Player");

        
        if (EnterPipe == "P1") // 파이프이동시 콜라이더가 충돌되지 않도록 바꿔준다.
        {
            playerGO.GetComponent<CapsuleCollider2D>().isTrigger = true;
            playerGO.GetComponentInChildren<CircleCollider2D>().enabled = false;
            playerGO.transform.position = playerPosition;
            playerGO.GetComponent<Rigidbody2D>().gravityScale = 0.0f;
            
            Player player = playerGO.GetComponent<Player>();
            
            player.bStopInput = true;
            
            EnterPipe = "";
        }

        GameObject.Find("Canvas").GetComponent<UIManager>().Coin(Coin);
        myAudio = GetComponent<AudioSource>();
        SceneName = SceneManager.GetActiveScene().name;
        //print(SceneManager.GetActiveScene().name); // 현재 씬 이름 구하기

        StartCoroutine("PastTime");
    }

    public static void SetPlayerPosition(Vector2 playerPositionV) // 스테틱 변수는 스테틱 함수에서 사용가능하다.
    {
        playerPosition = playerPositionV;
    }

    public static void SetPlayerFormation(Player.Formation formation)
    {
        //mformation = formation;
    }
    public static void SetEnterPipe(string EnterPipeName)
    {
        EnterPipe = EnterPipeName;
    }

    public static void SetCoin(int coin)
    {
        Coin = coin;
    }

    public static int GetCoin()
    {
        return Coin;
    }

    IEnumerator PastTime()
    {
        while (true)
        {
            pastTime++;

            
            //print(pastTime);
            yield return new WaitForSeconds(1.0f);
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.V)) // Pause 버튼
        {
            bPause = !bPause;
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("deadly");
            for(int i=0; i< enemies.Length; i++)
            {
                enemies[i].GetComponent<Gumba>().SetStop(bPause);
            }

            GameObject player = GameObject.FindGameObjectWithTag("Player");
            player.GetComponent<Player>().SetStop(bPause);


            GameObject[] fireballs = GameObject.FindGameObjectsWithTag("fireball");
            for(int i=0; i < fireballs.Length; i++)
            {
                fireballs[i].GetComponent<FireBall>().SetStop(bPause);
            }
            if (!bPause)
                StartCoroutine("PastTime");
            else
                StopCoroutine("PastTime");
        }

        Timer();
        

        if (playerGO.GetComponent<Player>().bDie == true)
        {
            BGMChanger(2, false);
        }
        else if (Player.bStarBGM == true)
        {
            BGMChanger(3, true);
        }
        else if (playerGO.GetComponent<Player>().bClear == true)
        {
            BGMChanger(4, false);
        }
        else if(bPause)
        {
            BGMChanger(5, false);
        }
        else if(bStop == false)
        {
            int bgmNumber;
            if (SceneName == "STAGE1-1a")
                bgmNumber = 0;
            else if (SceneName == "STAGE1-1b")
                bgmNumber = 1;
            else
                bgmNumber = 1;
            BGMChanger(bgmNumber, true);
        }
    }
    void OnDestroy()
    {
        
        //Debug.print("Destroy GameManager");
    }

  


    void Timer()
    {
        currentTime = StartTime - pastTime;
        GameObject.Find("Canvas").GetComponent<UIManager>().Timer(currentTime);
    }

    void BGMChanger(int BGMNumber, bool loop)
    {
        myAudio.loop = loop;
        if (loop == true)
            playTimes = 0;
        myAudio.clip = audioClips[BGMNumber];
        if(myAudio.isPlaying == false && playTimes == 0)
        {
            myAudio.Play();
            playTimes = 1;
   
        }
            
        
    }

    public void StopBGM()
    {
        myAudio.Stop();
        bStop = true;
    }
    // [씬 전환해도 변수 값 유지 하는 방법]
    // GameManager 클래스를 만든다. 메서드 외부에 "static int a = 1;"을 추가한다.
    // 씬에 빈 오브젝트를 만든다. 이름을 GM이라고 짓는다. GM에 GameManager 클래스를 컴포넌트로 넣는다.
    // 디버그 용으로 V키를 누르면 a값이 1증가하고 디버그로 a값을 출력하는 기능을 만든다.   
    // GM을 프리펩으로 만든다. 이 GM 프리펩을 Scene1과 Scene2에 똑같이 하이어라키에 넣어준다.
    // 씬을 전환하는 기능을 만든다.
    // 이제 씬을 전환하고 V키를 누르면 디버그 창에 a가 증가할 것이다. 씬이 전환되어도 a값은 1로 초기화 되지 않는다.
    // 이 GameManger 클래스는 게임의 간단한 데이터베이스로써 활용할 수 있다.
    


}
