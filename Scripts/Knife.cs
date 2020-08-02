using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Knife : MonoBehaviour
{

    public Vector2 direction;       // knife direction

    [SerializeField]
    private float speed;        //knife speed

    private Rigidbody2D myRigidbody;        // knife prefab

    
    
    // Start is called before the first frame update
    void Start()
    {
        myRigidbody = GetComponent <Rigidbody2D>();             // gets knife instance
    }
    void Update()
    {
        myRigidbody.velocity = direction * speed;           // moves knife
    }

    public void Initialize(Vector2 direction)
    {
        this.direction = direction;         // sets direction of knife
    }

    void OnBecameInvisible()
    {
        Destroy(gameObject);                // destroys knife when dissapears off screen
    }
}