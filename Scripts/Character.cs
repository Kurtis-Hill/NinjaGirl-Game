using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class Character : MonoBehaviour
{
    protected bool facingRight;     // checks player directionality

    [SerializeField]
    protected Transform knifePos;           // character knife pos

    [SerializeField]
    protected float movementSpeed;      // character movement speed

    [SerializeField]
    protected int health;       // character health

    [SerializeField]
    private GameObject kunaiPrefab;     // knife prefab refrence

    [SerializeField]
    private EdgeCollider2D swordCollider;       // character sword colider

    [SerializeField]
    protected List<string> damageSources;       // list of things that can hurt characters

    [SerializeField]
    protected float maxHitPoints;       // chcarter max hit points

    //new stuff
    public Image currentHP;     // on screen health
    public Text ratioText;      // on screen health text


   // public Vector2 StartPos { get; private set; }

    public abstract bool IsDead { get; }        

    public bool Attack { get; set; }

    public bool TakingDamage { get; set; }

    public Animator MyAnimator { get; private set; }        // properties used by characters

    public abstract IEnumerator TakeDamage();

    public abstract void Death();

    public EdgeCollider2D SwordCollider     // lets classes acces sword collider
    {
        get
        {
            return swordCollider;
        }

    }
    //public abstract int Damage { get; set; }

    // Start is called before the first frame update
    public virtual void Start()
    {       
        facingRight = true;                     // makes sure character faces correct direction
        MyAnimator = GetComponent<Animator>();  // Animator get charcter 
    }

    // Update is called once per frame
    void Update()
    {
        MyAnimator = GetComponent<Animator>();
    }

    //health
    protected virtual void UpdateHealth()   // updates health after collision
    {
        if (health >= 0)
        {
            float ratio = health / maxHitPoints;
            
        }
    }

    protected void ChangeDirection()        // changes character direction
    {
        facingRight = !facingRight;
        transform.localScale = new Vector3(transform.localScale.x*-1,transform.localScale.y,transform.localScale.z);
    }

    public virtual void ThrowKnife(int value)
    {
        if (facingRight)
        {
            GameObject tmp = (GameObject)Instantiate(kunaiPrefab, knifePos.position, Quaternion.Euler(new Vector3(0, 0, 0)));
            tmp.GetComponent<Knife>().Initialize(Vector2.right);
        }
        else
        {
            GameObject tmp = (GameObject)Instantiate(kunaiPrefab, knifePos.position, Quaternion.Euler(new Vector3(0, 0, 180)));
            tmp.GetComponent<Knife>().Initialize(Vector2.left);  // spawns knife the correct direction 
        }
    }

    public void MeleeAttack()
    {
        
        SwordCollider.enabled = true;       //turns sword collider on
       
    }
   

    public virtual void OnTriggerEnter2D(Collider2D other)  // check to se what player is colliding with and deals with collision correctly
    {
        if (damageSources.Contains(other.tag))
        {
            StartCoroutine(TakeDamage());
        }
    }
}
