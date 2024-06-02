using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour,IInteractable
{
    private SpriteRenderer spriteRenderer;
    public Sprite openSprite;
    public Sprite closeSprite;
    public bool isDone;
    private AudioDefination ad;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        ad = GetComponent<AudioDefination>();
    }

    private void OnEnable()
    {
        spriteRenderer.sprite = isDone ? openSprite : closeSprite;
    }
    public void TriggerAction()
    {
        Debug.Log("Open!");
        if(!isDone)
        {
            OpenChest();
            ad.PlayAudioClip();
        }
    }
    private void OpenChest()
    {
        spriteRenderer.sprite = openSprite;
        isDone = true;
        this.gameObject.tag = "Untagged";
    }
}
