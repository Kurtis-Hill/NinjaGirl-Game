using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public delegate void DeadEventHandler();        // dead event handler so enemy class knows when player dies

public class Player : Character
{
    public event DeadEventHandler Dead;

    private static Player instance;         // Player instance so other classes can acccess variables/ methods

    public static Player Instance       // get methods so outside interactin cannot set and change
    {
        get
        {
            if (instance == null)
            {
                instance = GameObject.FindObjectOfType<Player>();
            }
            return instance;
        }
    }

    //healthbar stuff
    //public Image currentHP;
    //public Text ratioText;
    //  public int maxHitPoints = 100;
    //public int shield;



    // was protected
    [SerializeField]
    private int playerStartHealth;

    [SerializeField]
    private bool airControl;

    [SerializeField]
    private Transform[] groundPoints; // game objects ont he players feet to detect ground

    [SerializeField]
    private float groundRadius; // checking to see how far player is away from ground

    [SerializeField]
    private LayerMask whatsIsGround;  // so the transforms know what ground is and what is enemy/ other stuff

    [SerializeField]
    private float jumpforce;

    

    [SerializeField]
    private float immortalTime;     

    private bool immortal = false;          // so the player isnt always immortal
   
    public Rigidbody2D MyRigidBody { get; set; }

    private SpriteRenderer spriteRenderer;          // render player sprite so it can flash when immortal

    public bool Slide { get; set; }

    public bool Jump { get; set; }

    public bool OnGround { get; set; }          // is the player on ground

    [SerializeField]
    private float climbSpeed;

    public bool OnLadder { get; set; }

    private Iuseable useable;       // used to allow access to ladder

    private Vector2 startPos;       // for respawn


    public override bool IsDead         // overrides character method 
    {
        get
        {
            if (health <= 0)
            {
                OnDead();
            }
            
            return health <= 0;
        }
    }

    public bool IsFalling           // check to see if the character is fallign down by checking y axis
    {
        get
        {
            return MyRigidBody.velocity.y < 0;
        }       
    }


    //private int playerDamage = 20;

    //public override int Damage
    //{
    //    get
    //    {
    //        return Damage;
    //    }

    //    set
    //    {
    //        Damage = playerDamage;
    //    }
    //}




    public override void Start()    // Called at the start of the game to load the following
    {
        base.Start();
        startPos = transform.position;
        MyRigidBody = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        OnLadder = false;
    }

    void Update()
    {       
        HandleInput();
        if (!TakingDamage && !IsDead)
        {
            if (transform.position.y <= -14f)
            {
                Death();
            }
        }
    }


    void FixedUpdate()
    {      
        if (!TakingDamage && !IsDead)
        {
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");
            OnGround = IsGrounded();
            HandleMovement(horizontal, vertical);
            Flip(horizontal);
            HandleLayers();
        }
        UpdateHealth();
    }
    //health stuff
    //private void UpdateHealth()
    //{
    //    float ratio = health / maxHitPoints;
    //    currentHP.rectTransform.localScale = new Vector3(ratio, 1, 1);
    //    ratioText.text = (ratio * 100).ToString();
    //}

    public void OnDead()    // called when the players health reaches 0 to call dead to send a dead event handler
    {
        if (Dead != null)
        {
            Dead();
        }
    }

    private void HandleMovement(float horizontal, float vertical)       // handles movement of the player and sets conditions so actions cannot be over lapped
    {

        if (IsFalling)
        {
            MyAnimator.SetBool("Land", true);
            gameObject.layer = 11;           
        }

        if (!Attack && !Slide && (OnGround || airControl))
        {
            MyRigidBody.velocity = new Vector2(horizontal * movementSpeed, MyRigidBody.velocity.y);
        }

        if (Jump && MyRigidBody.velocity.y == 0 && !OnLadder)
        {
            MyRigidBody.AddForce(new Vector2(0, jumpforce));
        }

        if (OnLadder)
        {
            MyAnimator.speed = vertical != 0 ? Mathf.Abs(vertical) : Mathf.Abs(horizontal);
            MyRigidBody.velocity = new Vector2(horizontal * climbSpeed, vertical * climbSpeed);
        }

        MyAnimator.SetFloat("speed", Mathf.Abs(horizontal));
    }

    

    private void HandleInput()  // sets controls for the player
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            MyAnimator.SetTrigger("Attack");
        }
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            MyAnimator.SetTrigger("Slide");
        }

        if (Input.GetKeyDown(KeyCode.Space) && !OnLadder && !IsFalling)
        {
            MyAnimator.SetTrigger("Jump");
        }

        if (Input.GetKeyDown(KeyCode.CapsLock))
        {
            MyAnimator.SetTrigger("Throw");
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            Use();
        }

        //if ()
        //{

        //}

        //if ()
        //{

        //}

        //if ()
        //{

        //}

        //if ()
        //{

        //}


    }


    private void Flip(float horizontal) // flips chcarter sprite depending on direction faced 
    {
        if (horizontal > 0 && !facingRight || horizontal < 0 && facingRight)
        {
            ChangeDirection();
        }
    }

        private bool IsGrounded()   // loops through ground point gmae object to see if the player is on the ground or not
        {
            if (MyRigidBody.velocity.y <= 0)
            {
                foreach (Transform point in groundPoints)
                {
                    Collider2D[] colliders = Physics2D.OverlapCircleAll(point.position, groundRadius, whatsIsGround);

                    for (int i = 0; i < colliders.Length; i++)
                    {
                        if (colliders[i].gameObject != gameObject)
                        {
                   //   Debug.Log("Grounded: "+ colliders[i].gameObject.name);
                        return true;
                        }
                    }

                }
            }
            return false;
    }

    private void HandleLayers()     // sets the layers for the player state machine so they can change layers depending on actions
    {
        if (!OnGround)
        {
            MyAnimator.SetLayerWeight(1, 1);
            MyAnimator.SetLayerWeight(0, 0);
        }
        else
        {
            MyAnimator.SetLayerWeight(1, 0);
            MyAnimator.SetLayerWeight(0, 1);
        }
    }

    public override void ThrowKnife(int value)      // overrides pre defined fucntiona and throws knife
    {
        if (!OnGround && value == 1 || OnGround && value == 0)
        {
            base.ThrowKnife(value);
        }

    }

    private IEnumerator IndicateImmortal()      // sets how many frames the sprtite render flashs after takign damage
    {
        while (immortal)
        {
            spriteRenderer.enabled = false;
            yield return new WaitForSeconds(.1f);
            spriteRenderer.enabled = true;
            yield return new WaitForSeconds(.1f);
        }
    }

    public override IEnumerator TakeDamage()        // works out damage done to player and begins immortal
    {
        if (!immortal)
        {
            health -= 10;
            if (!IsDead)
            {
                //health
               // UpdateHealth();
                MyAnimator.SetTrigger("Damage");
                immortal = true;
                StartCoroutine(IndicateImmortal());
                yield return new WaitForSeconds(immortalTime);
                immortal = false;
            }
            else
            {
                MyAnimator.SetLayerWeight(1, 0);
                MyAnimator.SetTrigger("Death");
                
            }           
        }
    }

    public override void Death()            // reswpans the player after dying and also resets values
    {
        MyRigidBody.velocity = Vector2.zero;
        MyAnimator.SetTrigger("Idle");
        health = playerStartHealth;
        transform.position = startPos;
    }

    private void Use()      //use for the ladder
    {
        if (useable != null)
        {
            useable.Use();
        }
    }


    public override void OnTriggerEnter2D(Collider2D other)     //allows ladder use when colliding with ladder
    {
        if (other.tag == "Useable")
        {
            useable = other.GetComponent<Iuseable>();
        }
        base.OnTriggerEnter2D(other);
    }

    private void OnTriggerExit2D(Collider2D other)      // makes sure the ladder caanot be used outside of box collider
    {
        if (other.tag == "Useable")
        {
            useable = null;
        }
    }

    protected override void UpdateHealth()      // updates player health after damage has been done and sets the health ratio at the top
    {
        float ratio = health / maxHitPoints;
        currentHP.rectTransform.localScale = new Vector3(ratio, 1, 1);
        ratioText.text = (ratio * 100).ToString();
    }
}
