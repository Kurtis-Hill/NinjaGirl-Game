﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolState : IEnemyState
{
    private Enemy enemy;
    private float patrolTimer;
    private float patrolDuration;


    public void Enter(Enemy enemy)
    {
        this.enemy = enemy;
        patrolDuration = UnityEngine.Random.Range(1, 10);
    }

    public void Execute()
    {
        //Debug.Log("Patroling");
        Patrol();
        enemy.Move();

        if (enemy.Target != null && enemy.InThrowRange)
        {
            enemy.ChangeState(new RangedState());
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

    private void Patrol()
    {       
        patrolTimer += Time.deltaTime;

        if (patrolTimer >= patrolDuration)
        {
            enemy.ChangeState(new IdleState());
        }
    }
}
