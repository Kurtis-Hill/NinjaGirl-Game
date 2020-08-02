using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySight : MonoBehaviour
{
    [SerializeField]
    private Enemy enemy;

    private void OnTriggerEnter2D(Collider2D other)     // checks collision box for player
    {
        if (other.tag == "Player")
        {
            enemy.Target = other.gameObject;
        }
    }
    private void OnTriggerExit2D(Collider2D other)      // returns target as null once hte collision box has exited
    {
        if (other.tag == "Player")
        {
            enemy.Target = null;
        }
    }
}
