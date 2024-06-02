using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XInput;

public class Sign : MonoBehaviour
{
    private PlayerInputControl playerInput;
    private Animator anim;
    public Transform playerTrans;
    public GameObject signSprite;
    private bool canPress;
    private IInteractable targetItem;
    private void Awake()
    {
        // anim = GetComponentInChildren<Animator>();
        anim = signSprite.GetComponent<Animator>();
        //playerTrans = GetComponent<Transform>();
        playerInput = new PlayerInputControl();
        playerInput.Enable();


    }
    private void OnEnable()
    {
        InputSystem.onActionChange += OnActionChange;
        playerInput.Gameplay.Confirm.started += OnConfirm;
    }

    private void OnDisable()
    {
        canPress = false;
    }

    private void Update()
    {
        signSprite.GetComponent<SpriteRenderer>().enabled = canPress;
        signSprite.transform.localScale = playerTrans.localScale;
    }

    private void OnConfirm(InputAction.CallbackContext context)
    {
        if(canPress)
        {
            targetItem.TriggerAction();
       //     GetComponent<AudioDefination>().PlayAudioClip();
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Interactable"))
        {
            canPress = true;
            targetItem = collision.GetComponent<IInteractable>();
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        canPress = false;
    }


    private void OnActionChange(object obj, InputActionChange actionChange)
    {
        if(actionChange == InputActionChange.ActionStarted)
        {
            //Debug.Log(((InputAction)obj).activeControl.device);
            var d = ((InputAction)obj).activeControl.device;
            switch (d.device)
            {
                case Keyboard:
                    anim.Play("keyboard");
                    break;
                case XInputControllerWindows:
                    anim.Play("xbox");
                    break;
            }
        }
    }

}
