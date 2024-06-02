using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SavePoint : MonoBehaviour,IInteractable
{
    [Header("广播")]
    public VoidEventSO saveDataEvent;
    [Header("变量参数")]
    public SpriteRenderer spriteRenderer;
    public GameObject lightObj;
    public Sprite darkSprite;
    public Sprite lightSprite;
    public bool isDone;
    private AudioDefination ad;

    private void Awake()
    {
        ad = GetComponent<AudioDefination>();
    }
    private void OnEnable()
    {
        spriteRenderer.sprite = isDone ? lightSprite : darkSprite;
        lightObj.SetActive(isDone);
    }
    public void TriggerAction()
    {
        if(!isDone)
        {
            isDone = true;
            spriteRenderer.sprite = lightSprite;
            lightObj.SetActive(true);
            saveDataEvent.RaiseEvent();
            //ToDo:baocunshuju
            this.gameObject.tag = "Untagged";
            ad.PlayAudioClip();
        }
    }

}
