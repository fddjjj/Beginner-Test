using System.Collections;
using System.Collections.Generic;
using System.Net.Mail;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D),typeof(Animator),typeof(PhysicalCheck))]
public class Enemy : MonoBehaviour
{
    [HideInInspector] public Rigidbody2D rb;
    [HideInInspector]public Animator anim;
    [HideInInspector]public PhysicalCheck physicalCheck;


    [Header("基本参数")]
    public float normalSpeed;
    public float chaseSpeed;
    [HideInInspector]public float currentSpeed;
    public Vector3 facedir;
    public Transform attacker;
    public float hurtForce;
    public Vector3 spwanPoint;

    [Header("检测")]
    public Vector2 centerOffest;
    public Vector2 checkSize;
    public float checkDistance;
    public LayerMask attackLayer;


    [Header("计时器")]
    public float waitTime;
    public float waitTimeCounter;
    public bool wait;
    public float lostTime;
    public float lostTimeCounter;




    [Header("状态")]
    public bool isHurt;
    public bool isDead;
    private BaseState currentState;
    protected BaseState patrolState;
    protected BaseState chaseState;
    protected BaseState skillState;


    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        physicalCheck = GetComponent<PhysicalCheck>();
        currentSpeed = normalSpeed;
        waitTimeCounter = waitTime;
        spwanPoint = transform.position;
    }

    private void OnEnable()
    {
        currentState = patrolState;
        currentState.OnEnter(this);
    }
    private void Update()
    {
        facedir = new Vector3(-transform.localScale.x, 0, 0);
        currentState.LogicUpdate();
        TimeCounter();
        
    }

    private void FixedUpdate()
    {
        if(!isHurt && !isDead && !wait) 
        {
            Move();
        }
        currentState.PhysicsUpdate();
    }

    private void OnDisable()
    {
        currentState.OnExit();
    }
    
    public virtual void Move() 
    {
        if(!anim.GetCurrentAnimatorStateInfo(0).IsName("SnailPreMove")&&!anim.GetCurrentAnimatorStateInfo(0).IsName("SnailReCover"))
            rb.velocity = new Vector2(currentSpeed * facedir.x * Time.deltaTime, rb.velocity.y);
    }
   /// <summary>
   /// 计时器 
   /// </summary>
    public void TimeCounter()
    {
        if (wait) 
        {
            waitTimeCounter -= Time.deltaTime;
            if(waitTimeCounter <= 0)
            {
                wait = false;
                waitTimeCounter = waitTime;
                transform.localScale = new Vector3(facedir.x, 1, 1);
            }
        }

        if (!FoundPlayer() && lostTimeCounter > 0)
        {
            lostTimeCounter -= Time.deltaTime;
        }
    }

    public virtual bool FoundPlayer()
    {
        return Physics2D.BoxCast(transform.position + (Vector3)centerOffest,checkSize ,0 ,facedir ,checkDistance , attackLayer); 
    }

    public void SwitchState(NPCState state)
    {
        var newState = state switch
        {
            NPCState.Patoal => patrolState,
            NPCState.Chase => chaseState,
            NPCState.Skill => skillState,
            _ => null
        };
        currentState.OnExit();
        currentState = newState;
        currentState.OnEnter(this);

    }

    public virtual Vector3 getNewPoint()
    {
        return transform.position;
    }

    #region 事件执行方法
    public void OnTakeDamage(Transform attackerTransform) 
    {
        attacker = attackerTransform;
        //转身
        if(attackerTransform.position.x - transform.position.x > 0)
            transform.localScale = new Vector3(-1,1,1);
        if (attackerTransform.position.x - transform.position.x < 0)
            transform.localScale = new Vector3(1,1,1);
        //Debug.Log("hurt");
        //受伤击退
        isHurt = true;
        anim.SetTrigger("hurt");
        Vector2 dir = new Vector2(transform.position.x - attackerTransform.position.x, 0).normalized;
        rb.velocity = new Vector2(0, rb.velocity.y);
        rb.AddForce(dir * hurtForce, ForceMode2D.Impulse);

        StartCoroutine(OnHurt(dir));
    }
    
    private IEnumerator OnHurt(Vector2 dir)
    {
        rb.AddForce(dir * hurtForce, ForceMode2D.Impulse);
        yield return new WaitForSeconds(0.5f);
        isHurt = false;

    }

    public void OnDead() 
    {
        gameObject.layer = 2;
        anim.SetBool("dead", true);
        isDead = true;
    }

    public void DestroyAfterAnimation() 
    {
        Destroy(this.gameObject);
    }
    #endregion 

    public virtual void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position + (Vector3)centerOffest + new Vector3(checkDistance * -transform.localScale.x,0),0.2f);

    }
}
