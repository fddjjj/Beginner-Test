using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoarPatrolState : BaseState
{
    public override void OnEnter(Enemy enemy)
    {
        currentEnemy = enemy;
        currentEnemy.currentSpeed  = currentEnemy.normalSpeed;

    }
    public override void LogicUpdate()
    {
        if(currentEnemy.FoundPlayer())
        {
            currentEnemy.SwitchState(NPCState.Chase);
        }

        if (!currentEnemy.physicalCheck.isGround ||(currentEnemy.physicalCheck.touchLeftWall && currentEnemy.facedir.x < 0) ||  (currentEnemy.physicalCheck.touchRightWall && currentEnemy.facedir.x > 0))
        {
           // Debug.Log("��ʼwait");
            currentEnemy.wait = true;
            currentEnemy.anim.SetBool("walk", false);
        }else
        {
            currentEnemy.anim.SetBool("walk", true);
        }
    }

     public override void PhysicsUpdate()
    {
        
    }

    public override void OnExit()
    {
        currentEnemy.anim.SetBool("walk", false);
    }

   
}
