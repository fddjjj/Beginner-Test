using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoarChaseState : BaseState
{
    public override void OnEnter(Enemy enemy)
    {
        currentEnemy = enemy;
        currentEnemy.currentSpeed = currentEnemy.chaseSpeed;
        currentEnemy.anim.SetBool("run", true);

    }
    public override void LogicUpdate()
    {
        if(currentEnemy.lostTimeCounter <= 0)
        {
            currentEnemy.SwitchState(NPCState.Patoal);
        }
        if (!currentEnemy.physicalCheck.isGround || (currentEnemy.physicalCheck.touchLeftWall && currentEnemy.facedir.x < 0) ||  (currentEnemy.physicalCheck.touchRightWall && currentEnemy.facedir.x > 0))
        {
            currentEnemy.transform.localScale = new Vector3(currentEnemy.facedir.x, 1, 1);
        }
    }

    public override void PhysicsUpdate()
    {
        
    }

    public override void OnExit()
    {
        currentEnemy.lostTimeCounter = currentEnemy.lostTime;  
        currentEnemy.anim.SetBool("run", false);
    }

    

}
