using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Character
{
    private IEnemyState currentState;


    public GameObject Target { get; set; }

    [SerializeField]
    private float meleeRange;       // checks players range

    [SerializeField]
    private float throwRange;          // range enemy will throw knifes at player

    [SerializeField]
    private Vector3 startPos;       // starting posision for eahc enemy

    [SerializeField]
    private Transform leftEdge;     // left edge so they dont fall off

    [SerializeField]
    private Transform rightEdge;    // left edge so they dont fall off

    [SerializeField]
    protected int enemyStartHealth; // enemy start health


    public bool InMeleeRange    // meele range property with condition ot chek for enemys
    {
        get
        {
            if (Target != null)
            {
                return Vector2.Distance(transform.position, Target.transform.position) <= meleeRange;
            }
            return false;
        }
    }

    public bool InThrowRange    // checks to see if player is whithin throw range and return result
    {
        get
        {
            if (Target != null)
            {
                return Vector2.Distance(transform.position, Target.transform.position) <= throwRange;
            }
            return false;
        }
    }

    public override bool IsDead     // returns players health when accessed
    {
        get
        {
            return health <= 0;
        }
    }

    public static object Instance { get; internal set; }        // Enemy instance


    public bool FacingRight { get; set; }       // reveses facing right function


    //private int enemyDamage = 10;

    //public override int Damage
    //{
    //    get
    //    {
    //        return Damage;
    //    }

    //    set
    //    {
    //        Damage = enemyDamage;
    //    }
    //}





    public override void Start()
    {
        base.Start();
        Player.Instance.Dead += new DeadEventHandler(RemoveTarget);
        ChangeState(new IdleState());

        facingRight = FacingRight;
    }

    public void RemoveTarget()      // when player loses player collision box
    {
        Target = null;
        ChangeState(new PatrolState());
    }

    private void LookAtTarget() // when player enters enemy collisiion box face the correct direction
    {
        if (Target != null)
        {
            float xDir = Target.transform.position.x - transform.position.x;

            if (xDir < 0 && facingRight || xDir >0 && !facingRight)
            {
                ChangeDirection();
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsDead)
        {
            if (!TakingDamage)
            {
                currentState.Execute();
            }
            
            LookAtTarget();
        }
        UpdateHealth();
    }

    public void ChangeState(IEnemyState newState) // makes sure enemy can change state
    {
        if (currentState !=null)
        {
            currentState.Exit();
        }

        currentState = newState;

        currentState.Enter(this);
    }

    public void Move()      // enemy movement
    {
        if (!Attack)
        {
            if ((GetDirection().x > 0 && transform.position.x < rightEdge.position.x) || GetDirection().x < 0 && transform.position.x > leftEdge.position.x)
            {
                MyAnimator.SetFloat("speed", 1);

                transform.Translate(GetDirection() * (movementSpeed * Time.deltaTime));
            }
            else if (currentState is PatrolState)
            {
                ChangeDirection();
            } 
            else if (currentState is RangedState)
            {
                Target = null;
                ChangeState(new IdleState());
            }
        }
    }

    public Vector2 GetDirection()       // checks direcetion
    {
        return facingRight ? Vector2.right : Vector2.left;
    }

    public override void OnTriggerEnter2D(Collider2D other)     // overrides method set in character class
    {
        {
            base.OnTriggerEnter2D(other);
            currentState.OnTriggerEnter(other);
        }
    }

    public override IEnumerator TakeDamage()        // overrides method set in character class
    {
        health -= 10;

        if (!IsDead)
        {
            UpdateHealth();
            MyAnimator.SetTrigger("Damage");

            // if knife hits
            MyAnimator.SetTrigger("Throw");

            //if (damageSources.Contains(other.tag))
            //{
            //    // if sword hits  MyAnimator.SetTrigger("Attack");
            //}
        }
        else
        {
            MyAnimator.SetTrigger("Death");
            yield return null;
        }
    }

    public override void Death()   // multiple death options either destroy or respawn
    {
        //THIS WILL RESPAWN ENEMY (Change times in Enemy death animator)
        MyAnimator.SetTrigger("Idle");
        MyAnimator.ResetTrigger("Death");
        health = enemyStartHealth;
        transform.position = startPos;
        Target = null;
        // this will destroy this enemy game object upon death NO RESPAWN    
        //  Destroy(gameObject);      
    }


    
}
