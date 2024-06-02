using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Attack : MonoBehaviour
{
    [Header("»ù´¡ÊôÐÔ")]
    public int damage;
    public float attackRange;
    public float attackRate;
    private void OnTriggerStay2D(Collider2D collision)
    {
        collision.GetComponent<Character>()?.TackDamage(this);
    }
}
