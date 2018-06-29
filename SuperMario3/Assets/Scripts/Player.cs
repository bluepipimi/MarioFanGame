using UnityEngine;
using System.Collections;
//using UnityStandardAssets.CrossPlatformInput;

    // 플레이어 스프라이트르 center를 bottom으로 통일했음 : 변신시 높이 문제를 없애기 위해
public class Player : MonoBehaviour {

    public enum Formation
    {
        MARIO, SUPERMARIO, FLOWERMARIO,
    }

    public static Formation marioForm; // 씬이 전환되어도 변신을 유지함.
    private Rigidbody2D myRigidBody;
    private Animator myAnimator;
    private AudioSource myAudio;
    public bool facingRight; // 오른쪽을 바라보고 있는가
    private CapsuleCollider2D myCapsule;
    private CircleCollider2D myCircle;
    private SpriteRenderer mySprite;

    public RuntimeAnimatorController[] animControllers;

    [SerializeField]
    private float movementSpeed;
    private float directionSpeed;
    [SerializeField]
    private float jumpForce;

    bool isGrounded;
    public Transform foot; // 발위치를 지정하여 땅에 닿음을 측정한다.
    public float FootRadius;
    public LayerMask ground;

    public bool bStar;
    public static bool bStarBGM;
    public static float InvincibleTimer = 10.0f;
    private float InvincibleStartTime;
    private float invincibleCurrentTime;

    public Transform firePosition; // 불꽃 발사 위치
    public GameObject fireball;
    [SerializeField]
    private Vector2 velocity;

    [SerializeField]                    // [SerializeField]는 인스펙터창에 대상을 넣어주어야 기능이 동작한다.
    private AudioClip jumpSoundClip;
    [SerializeField]
    private AudioClip shootSoundClip;
    [SerializeField]
    private AudioClip coinSoundClip;
    [SerializeField]
    private AudioClip powerUpSoundClip;

    [SerializeField]
    private UIManager ui; // canvas를 여기에 넣어준다.

    public int coin;

    private bool bJumping;
    public bool bCrouch;    // 웅크리기
    public bool bGoRight;   // 오른쪽으로 버튼을 눌러 움직이고 있는가
    public bool bStopInput; // 입력금지
    public bool bInThePipe;
    public bool bDie;      // 죽었는가
    private bool DieFunc;  // Die 함수에 들어갔는가
    public bool bFlagPollTouch; // 골대 깃발에 닿았는가
    public bool bClear;    // 스테이지를 클리어 하였는가
    bool bPause;
    public Transform punch;


    // Use this for initialization
	void Start () {
        myRigidBody = GetComponent<Rigidbody2D>();
        myAnimator = GetComponent<Animator>();
        myAudio = GetComponent<AudioSource>();
        myCapsule = GetComponent<CapsuleCollider2D>();
        myCircle = GetComponentInChildren<CircleCollider2D>(); // 자식객체에서 컴포넌트를 가져온다.
        mySprite = GetComponent<SpriteRenderer>();
        bCrouch = false;
        bGoRight = false;
        facingRight = true;

        coin = GameManager.GetCoin();
        if (ui)
            ui.Coin(coin);
        ChangeFormation();

        if(bStar)
            StartCoroutine(StarEffect(true));
    }
	
	// Update is called once per frame
	void Update () {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        //float horizontal = CrossPlatformInputManager.GetAxis("Horizontal");
        //float vertical = CrossPlatformInputManager.GetAxis("Vertical");

        // test code
        if(Input.GetKeyDown(KeyCode.I)) // 마리오로 변신
        {
            marioForm = Formation.MARIO;
            ChangeFormation();
        }

        if(Input.GetKeyDown(KeyCode.O)) // 플라워마리오로 변신
        {
            marioForm = Formation.SUPERMARIO;
            ChangeFormation();
        }

        if(Input.GetKeyDown(KeyCode.P))
        {
            marioForm = Formation.FLOWERMARIO;
            ChangeFormation();
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            bStar = true;
            InvincibleStartTime = Time.time;
            StartCoroutine(StarEffect(true));
            
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            bStar = false;
            StopAllCoroutines();
            mySprite.color = Color.white;
        }

        if (bStar)
        {
            invincibleCurrentTime = Time.time - InvincibleStartTime;
            if (invincibleCurrentTime > InvincibleTimer)
            {
                bStar = false;
                StopAllCoroutines();
                mySprite.color = Color.white;
                invincibleCurrentTime = 0;
            }
        }

        if (bDie)
        {
            Die();
        }
        else
        {
            HandleMovement(horizontal, vertical);
            Flip(horizontal);
        }

        if (this.transform.position.y <= -100)
        {
            Regen();
        }

        //print(movementSpeed);
        //print(myRigidBody.velocity.x);
    }

    public void SetStop(bool stop)
    {
        bPause = stop;

        if (bPause)
        {
            myRigidBody.gravityScale = 0;
            myRigidBody.velocity = new Vector3(0, 0);
            myAnimator.speed = 0;
            bStopInput = true;
        }
        else
        {
            myRigidBody.gravityScale = 3.0f;
            myAnimator.speed = 1;
            bStopInput = false;
        }
    }

    IEnumerator StarEffect(bool end)
    {
        float waitTime;

        
        while(end)
        {
            if (InvincibleTimer - invincibleCurrentTime < 3.0f)
            {
                bStarBGM = false;
                waitTime = 0.5f;
            }
            else
            {
                bStarBGM = true;
                waitTime = 0.1f;
            }
                

            mySprite.color = new Color(255, 0, 100);
            yield return new WaitForSeconds(waitTime);

            mySprite.color = new Color(0, 255, 100);
            yield return new WaitForSeconds(waitTime);

            mySprite.color = new Color(100, 100, 255);
            yield return new WaitForSeconds(waitTime);
        }

        yield return null;
    }

    public void ChangeFormation()
    {
        if (marioForm == Formation.MARIO)
        {
            // Animator의 AnimationController 교체
            myAnimator.runtimeAnimatorController = animControllers[0]; // "mario"
            myCapsule.offset = new Vector2(0, 0.37f);
            myCapsule.size = new Vector2(0.63f, 0.76f);
            punch.localPosition = new Vector2(0.16f, 0.68f);
        }
        else if (marioForm == Formation.SUPERMARIO)
        {
            myAnimator.runtimeAnimatorController = animControllers[1]; // "Player"
            punch.localPosition = new Vector2(0.16f, 1.25f);

        }
        else if (marioForm == Formation.FLOWERMARIO)
        {
            myAnimator.runtimeAnimatorController = animControllers[2]; // "Player"
            punch.localPosition = new Vector2(0.16f, 1.25f);

        }
    }

    private void Die()
    {
        if (DieFunc == false)
        {
            DieFunc = true;
            myAnimator.runtimeAnimatorController = animControllers[0];
            
            myAnimator.SetBool("Die", true);
            myRigidBody.velocity = new Vector2(0, 0);
            myRigidBody.gravityScale = 2.0f;
            myRigidBody.AddForce(new Vector2(0, 500.0f));
            myCapsule.enabled = false;
            myCircle.enabled = false;
            mySprite.sortingOrder = 2;
            marioForm = Formation.MARIO;
            ChangeFormation();
        }
    }
    
    private void shoot()
    {
        myAudio.clip = shootSoundClip;
        myAudio.Play();
        myAnimator.SetTrigger("shoot");
        int shootDirection;

        if (facingRight)
            shootDirection = 1;
        else
            shootDirection = -1;

        GameObject ob = (GameObject)Instantiate(
            fireball, firePosition.position, Quaternion.identity);
        ob.transform.localScale = new Vector3(-shootDirection, 1, 1);
        
        ob.GetComponent<Rigidbody2D>().velocity = 
            new Vector2(velocity.x * transform.localScale.x, velocity.y);
    }

    private void HandleMovement(float horizontal, float vertical)
    {
        if (bInThePipe == false)
            isGrounded = Physics2D.OverlapCircle(foot.transform.position, FootRadius, ground);
        else
            isGrounded = true;

        if (isGrounded) // On Land
        {
            if (marioForm == Formation.SUPERMARIO || marioForm == Formation.FLOWERMARIO)
            {
                myAnimator.SetLayerWeight(1, 0);

                if (bCrouch) // 웅크렸을 때
                {
                    myAnimator.SetBool("bCrouch", true);
                    myCapsule.offset = new Vector2(0, 0.43f);
                    myCapsule.size = new Vector2(0.63f, 0.9f);
                }
                else // 서 있을 때
                {
                    myAnimator.SetBool("bCrouch", false);
                    myCapsule.offset = new Vector2(0, 0.67f);
                    myCapsule.size = new Vector2(0.63f, 1.37f);

                }
            }
            else if (marioForm == Formation.MARIO)
            {
                //myAnimator.SetLayerWeight(1, 0);
                myAnimator.SetBool("jump", false);
            }
            bJumping = false;
            myCircle.enabled = false;
        }
        else // Jumping
        {
            if (marioForm == Formation.MARIO)
            {
                //myAnimator.SetLayerWeight(1, 0);
                myAnimator.SetBool("jump", true);
            }
            else if (marioForm == Formation.SUPERMARIO || marioForm == Formation.FLOWERMARIO)
            {
                if (bFlagPollTouch == false)
                {
                    myAnimator.SetLayerWeight(1, 1);
                }
                else
                {
                    myAnimator.SetLayerWeight(1, 0); // 스테이지 클리어시 애니메이션을 보여주기 위해 이렇게 한다.
                    print("c");
                }

                myAnimator.SetFloat("velocityY", myRigidBody.velocity.y);

                if (myAnimator.GetBool("bCrouch") == true) // 웅크렸을 때
                {
                    myCapsule.offset = new Vector2(0, 0.43f);
                    myCapsule.size = new Vector2(0.63f, 0.9f);
                    punch.localPosition = new Vector2(0, 0.8f);
                }
                else 
                {
                    myCapsule.offset = new Vector2(0, 0.67f);
                    myCapsule.size = new Vector2(0.63f, 1.37f);
                    punch.localPosition = new Vector2(0.16f, 1.25f);

                }
            }
            bJumping = true;

            if (myRigidBody.velocity.y > 0)
                myCircle.enabled = true;
            else
                myCircle.enabled = false;
        }
        //print(myRigidBody.velocity.y);

        if (bStopInput) return;

        if (Input.GetButton("Fire1")) // x button
        {
            if (movementSpeed < 8.0f)
                movementSpeed += 0.1f;
            else
                movementSpeed = 8.0f;
        }
        else
        {
            if (movementSpeed < 3.9f)
                movementSpeed += 0.1f;
            else if (movementSpeed > 4.1f)
                movementSpeed -= 0.1f;
            else
                movementSpeed = 4;
        }

        myAnimator.speed = movementSpeed / 4.0f;

        //if (horizontal > 0.0f)
        //{
        //    if (directionSpeed < 4.0f)
        //        directionSpeed += 0.1f;
        //    else
        //        directionSpeed = 4.01f;
        //}
        //else if (horizontal < 0.0f)
        //{
        //    if (directionSpeed > -4.0f)
        //        directionSpeed -= 0.1f;
        //    else
        //        directionSpeed = -4.1f;
        //}
        //else
        //{
        //    if (directionSpeed > 2.0f)
        //        directionSpeed -= 2.0f;
        //    else if (directionSpeed < -2.0f)
        //        directionSpeed += 2.0f;
        //    else
        //        directionSpeed = 0;
        //}
            
        
        

        myRigidBody.velocity = new Vector2(horizontal * movementSpeed, myRigidBody.velocity.y);
        myAnimator.SetFloat("speed", Mathf.Abs(horizontal));

        if (vertical < 0.0f && !bJumping)
        {
            bCrouch = true;
            if (movementSpeed > 0)
                movementSpeed -= 0.1f;
            else
                movementSpeed = 0;
        }
        else
        {
            bCrouch = false;
            
        }
            

        if (horizontal <= 0.0f)
            bGoRight = false;
        else
            bGoRight = true;

        if (Input.GetButtonDown("Jump")// || CrossPlatformInputManager.GetButtonDown("Jump")  
           && !bStopInput && isGrounded == true)
        {
            if(!myAudio.isPlaying)
            {
                myAudio.clip = jumpSoundClip;
                myAudio.Play();
            }
            
            myRigidBody.AddForce(new Vector2(0, jumpForce));
        }
        if(Input.GetButtonUp("Jump") && myRigidBody.velocity.y > 1.0f) // 점프버튼을 놓으면 가속도를 줄여서 가변높이 점프가 가능하다.
        {
            myRigidBody.velocity -= new Vector2(0, 5.0f);
        }
        if (marioForm == Formation.FLOWERMARIO && Input.GetButtonDown("Fire1") && !bStopInput && !bCrouch) //|| CrossPlatformInputManager.GetButtonDown("Fire"))
        {
            shoot();
        }
    }

    private void Flip(float horizontal)
    {
        if (bStopInput)
            return;
        if (horizontal > 0 && !facingRight || horizontal < 0 && facingRight)
        {
            ChangeDirection();
        }
    }

    private void ChangeDirection()
    {
        facingRight = !facingRight;
        transform.localScale = new Vector3(transform.localScale.x * -1, 1, 1);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(foot.transform.position, FootRadius);
    }

    void Regen()
    {
        DieFunc = false;
        myAnimator.SetBool("Die", false);
        bDie = false;
        myRigidBody.velocity = new Vector2(0, 0);
        myRigidBody.gravityScale = 3.0f;
        transform.position = new Vector2(9,3);
        myCapsule.enabled = true;
        myCircle.enabled = true;
        mySprite.sortingOrder = -1;
        
        
    }

    public void GetCoin()
    {
        myAudio.clip = coinSoundClip;
        myAudio.Play();
        coin++;
        if (ui)
            ui.Coin(coin);
        GameManager.SetCoin(coin);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
    
        if(collision.tag == "DieArea")
        {
            bDie = true;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "deadly")
        {

            GameObject gumba = collision.gameObject;
            if(gumba.GetComponent<Gumba>().weakness.position.y > foot.position.y && bStar == false)
                bDie = true;
        }

        if(collision.gameObject.tag == "mushroom")
        {
            Destroy(collision.gameObject);
            marioForm = Formation.SUPERMARIO;
            myAudio.clip = powerUpSoundClip;
            myAudio.Play();
            ChangeFormation();
        }

        if (collision.gameObject.tag == "flower")
        {
            Destroy(collision.gameObject);
            marioForm = Formation.FLOWERMARIO;
            myAudio.clip = powerUpSoundClip;
            myAudio.Play();
            ChangeFormation();
        }

        if(collision.gameObject.tag == "star")
        {
            Destroy(collision.gameObject);
            bStar = true;
            InvincibleStartTime = Time.time;
            StartCoroutine(StarEffect(true));
        }
    }
}
