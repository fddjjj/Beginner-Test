using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("�����¼�")]
    public SceneLoadEventSO loadEvent;
    public VoidEventSO afterLoadSceneEvent;
    public VoidEventSO loadDataEvent;
    public VoidEventSO backToMenuEvent;


    public PlayerInputControl inputControl;
    private Rigidbody2D rb;
    private CapsuleCollider2D coll;
    private PlayerAnimation playerAnimation;
    private Character character;

    public Vector2 inputDirection;
    private PhysicalCheck physicalCheck;
    [Header("��������")]
    public float speed;
    public float jumpForce;
    public float wallJumpForce;
    public float runSpeed;
    public float hurtForce;
    public float slideDistance;
    public float slideSpeed;
    public int slidePowerCost;

    public float walkSpeed => runSpeed / 2.5f;
    
    private Vector2 originalOffset;
    private Vector2 originalSize;
    [Header("״̬")]
    public bool isCrouch;
    public bool isHurt;
    public bool isDead;
    public bool isAttack;
    public bool wallJump;
    public bool isSilde;

    [Header("����")]
    public PhysicsMaterial2D normal;
    public PhysicsMaterial2D wall;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        physicalCheck = GetComponent<PhysicalCheck>();
        coll = GetComponent<CapsuleCollider2D>();
        inputControl = new PlayerInputControl();
        playerAnimation = GetComponent<PlayerAnimation>();
        character = GetComponent<Character>();
        originalOffset = coll.offset;
        originalSize = coll.size;


        inputControl.Gameplay.Jump.started += Jump;

        #region ǿ����·
        runSpeed = speed;
        inputControl.Gameplay.WalkButton.performed += ctx =>
        {
            if (physicalCheck.isGround)
                speed = walkSpeed;
        };
        inputControl.Gameplay.WalkButton.canceled += ctx => 
        {
           // if(physicalCheck.isGround)
                speed = runSpeed;
        };
        #endregion

        //����
        inputControl.Gameplay.Attack.started += PlayerAttack;

        inputControl.Gameplay.Slide.started += Slide;
        inputControl.Enable();
    }

 

    private void OnEnable()
    {
        
        loadEvent.LoadRequestEvent += OnLoadEvent;
        afterLoadSceneEvent.OnEventRaised += OnAfterLoadSceneEvent;
        loadDataEvent.OnEventRaised += OnLoadDataEvent;
        backToMenuEvent.OnEventRaised += OnLoadDataEvent;
    }



    private void OnDisable()
    {
        inputControl.Disable();
        loadEvent.LoadRequestEvent -= OnLoadEvent;
        afterLoadSceneEvent.OnEventRaised -= OnAfterLoadSceneEvent;
        loadDataEvent.OnEventRaised -= OnLoadDataEvent;
        backToMenuEvent.OnEventRaised -= OnLoadDataEvent;
    }


    void Update()
    {
        inputDirection = inputControl.Gameplay.Move.ReadValue<Vector2>();
        CheckState();

    }
    private void FixedUpdate()
    {
        if(!isHurt&&!isAttack)
        {
            Move();
        }
    } 
    
    private void OnLoadDataEvent()
    {
        //Debug.Log("�ؼ���");
        isDead = false;
    }
    //�������ع�������ֹͣ����
    private void OnLoadEvent(GameSceneSO arg0, Vector3 arg1, bool arg2)
    {
        inputControl.Gameplay.Disable();
    }
    //��������������������ÿ���
    private void OnAfterLoadSceneEvent()
    {
        inputControl.Gameplay.Enable();
    }

    public void Move() 
    {
        //�����ƶ�
        if(!isCrouch && !wallJump)
            rb.velocity = new Vector2(inputDirection.x*speed*Time.deltaTime, rb.velocity.y);
        //���﷭ת
        int facedir = (int)transform.localScale.x;
        if (inputDirection.x > 0)   facedir = 1;
        else if(inputDirection.x < 0) facedir = -1;

        transform.localScale = new Vector3(facedir, 1, 1);
        //�¶�
        isCrouch = inputDirection.y < -0.5f && physicalCheck.isGround;
        if(isCrouch )
        {
            //�޸���ײ���С��λ��
            rb.velocity = new Vector2(0,rb.velocity.y);
            coll.offset = new Vector2(-0.05f,0.45f);
            coll.size = new Vector2(0.7f,0.9f);
        }
        else
        {
            //��ԭ
            coll.size = originalSize;
            coll.offset = originalOffset;
        }
    }
    private void Jump(InputAction.CallbackContext context)
    {
        if (physicalCheck.isGround)
        {
            rb.AddForce(transform.up * jumpForce, ForceMode2D.Impulse);
            GetComponent<AudioDefination>().PlayAudioClip();
            //��ϻ���Э��
            if(isSilde)
            {
                isSilde = false;
                gameObject.layer = LayerMask.NameToLayer("player");
                character.invulnerable = false;
                StopAllCoroutines();
            }
            
        }
        else if (physicalCheck.onWall)
        {
            rb.AddForce(new Vector2(-inputDirection.x , 2f)*wallJumpForce, ForceMode2D.Impulse);
            wallJump = true;
        }
    }

    private void PlayerAttack(InputAction.CallbackContext context)
    {
        playerAnimation.PlayAttack();
        isAttack = true;
    }
    private void Slide(InputAction.CallbackContext context)
    {
        if (!isSilde && physicalCheck.isGround && character.currentPower >= slidePowerCost)
        {
            isSilde = true;
            var targetPos = new Vector3(transform.position.x + slideDistance * transform.localScale.x, transform.position.y);
            gameObject.layer = LayerMask.NameToLayer("enemy");
            //�������ṩһ����ʱ�޵�
            character.invulnerableCounter = character.invulnerableDuration;
            character.invulnerable = true;

            StartCoroutine(TriggerSlide(targetPos));

            character.OnSlide(slidePowerCost);
        }
            


    }

    private IEnumerator TriggerSlide(Vector3 target)
    {
        do
        {
            yield return null;
            if(!physicalCheck.isGround)
                break;
            if(physicalCheck.touchLeftWall && transform.localScale.x < 0f || physicalCheck.touchRightWall && transform.localScale.x > 0f)
            {
                isSilde = false;
                break;
            }
            rb.MovePosition(new Vector2(transform.position.x + transform.localScale.x * slideSpeed, transform.position.y));

        } while (MathF.Abs(target.x - transform.position.x) > 0.1f);
        isSilde = false;
        character.invulnerable = false;
        gameObject.layer = LayerMask.NameToLayer("player");
    }
    public void GetHurt(Transform attacker)
    {
        isHurt = true;
        rb.velocity = Vector2.zero;
        Vector2 dir = new Vector2((transform.position.x -  attacker.position.x),0).normalized;
        rb.AddForce(dir * hurtForce, ForceMode2D.Impulse);
    }
    public void playDead()
    {
        isDead = true;
        inputControl.Gameplay.Disable();

    }
    private void CheckState()
    {
        coll.sharedMaterial = physicalCheck.isGround ? normal : wall;
        if(physicalCheck.onWall)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y / 2f);
        }else
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y);
        }
        if(wallJump && rb.velocity.y < 5f)
        {
            wallJump = false;
        }
    }
}
