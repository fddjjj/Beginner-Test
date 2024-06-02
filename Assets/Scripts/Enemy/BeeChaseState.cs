using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeeChaseState : BaseState
{
    private Attack attack;
    private Vector3 target;
    private Vector3 moveDir;
    private bool isAttack;
    private float attackRateCounter = 0;
    public override void OnEnter(Enemy enemy)
    {
        currentEnemy = enemy;
        currentEnemy.currentSpeed = currentEnemy.chaseSpeed;
        attack = enemy.GetComponent<Attack>();

        currentEnemy.lostTimeCounter = currentEnemy.lostTime;
        currentEnemy.anim.SetBool("chase", true);
    }
    public override void LogicUpdate()
    {
        if (currentEnemy.lostTimeCounter <= 0)
        {
            currentEnemy.SwitchState(NPCState.Patoal);
        }
        target = new Vector3(currentEnemy.attacker.position.x, currentEnemy.attacker.position.y + 1f,0);

        //�жϹ�������
        if(Mathf.Abs(target.x - currentEnemy.transform.position.x) <= attack.attackRange && Mathf.Abs(target.y - currentEnemy.transform.position.y) <= attack.attackRange)
        {
            //����
            isAttack = true;
            if (!currentEnemy.isHurt)
            {           
                currentEnemy.rb.velocity = Vector2.zero;
            }

            //��ʱ��
            attackRateCounter-= Time.deltaTime;
            if(attackRateCounter <=0)
            {
                attackRateCounter = attack.attackRate;
                currentEnemy.anim.SetTrigger("attack");
            }

        }else
        {
            isAttack=false;
        }



        //���﷭ת
        moveDir = (target - currentEnemy.transform.position).normalized;

        if (moveDir.x > 0)
        {
            currentEnemy.transform.localScale = new Vector3(-1, 1, 1);
        }
        if (moveDir.x < 0)
        {
            currentEnemy.transform.localScale = new Vector3(1, 1, 1);
        }
    }


    public override void PhysicsUpdate()
    {
        if ( !currentEnemy.isHurt && !currentEnemy.isDead && !isAttack)
        {
            currentEnemy.rb.velocity = moveDir * currentEnemy.currentSpeed * Time.deltaTime;
        }
        else if(isAttack && !currentEnemy.isHurt)
        {
            currentEnemy.rb.velocity = Vector2.zero;
        }
    }

    public override void OnExit()
    {
        currentEnemy.anim.SetBool("chase", false);
    }

}
