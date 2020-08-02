using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : IEnemyState
{

    private Enemy enemy;

    private float idleTimer;

    private float idleDuration;




    public void Enter(Enemy enemy)
    {
        idleDuration = UnityEngine.Random.Range(1, 5);
        this.enemy = enemy;

    }

    public void Execute()
    {
        //   Debug.Log("Im Idleing");
        Idle();

        if (enemy.Target != null)
        {
            enemy.ChangeState(new PatrolState());
        }
    }

    public void Exit()
    {

    }

    public void OnTriggerEnter(Collider2D other)
    {
        if (other.tag == "Knife")
        {
            enemy.Target = Player.Instance.gameObject;
        }
    }

    private void Idle()
    {
        if (enemy.gameObject.layer == 10)
        {
            idleDuration = 10000;

        }


        enemy.MyAnimator.SetFloat("speed", 0);
            idleTimer += Time.deltaTime;

            if (idleTimer >= idleDuration)
            {
                enemy.ChangeState(new PatrolState());
            }
        

    }
}
