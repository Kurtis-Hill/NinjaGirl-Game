using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformMovement : MonoBehaviour
{
    private Player player;

    private Character character;

    private Enemy enemy;

    private Vector3 posA;

    private Vector3 posB;

    private Vector3 nexPos;  // platform movement vectoors

    [SerializeField]
    private float speed;        // platform movement speed

    [SerializeField]
    private Transform childTransform;   // game objects entering platform

    [SerializeField]
    private Transform transformB; // transform postion B

    

  
    // Start is called before the first frame update
    void Start()        
    {
        posA = childTransform.localPosition;
        posB = transformB.localPosition;
        nexPos = posB;
        
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }

    private void Move() // begins movement of platforms
    {
        childTransform.localPosition = Vector3.MoveTowards(childTransform.localPosition, nexPos, speed * Time.deltaTime);

        if (Vector2.Distance(childTransform.localPosition,nexPos) <= 0.1)
        {
            ChangeDirectionP();
        }
    }

    private void ChangeDirectionP()     // changing direction once first target has been hit
    {
        nexPos = nexPos != posA ? posA : posB;
    }

    private void OnCollisionEnter2D(Collision2D other)      // making sure when player lands on the platform it sets it tot he same layer so the player stays on the platform
    {
        if (Player.Instance.IsFalling == false) 
        {

            if (other.gameObject.tag == ("Player"))
            {
                other.gameObject.layer = 10;
                other.transform.SetParent(childTransform);
            }
        }
        if (other.gameObject.tag == ("Enemy"))  // making sure enemys stay on the platform
        {
            
            other.gameObject.layer = 10;
            other.transform.SetParent(childTransform);


        }

    }

        private void OnCollisionExit2D(Collision2D other)
        {
         other.transform.SetParent(null);
        }
}
