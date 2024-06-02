using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Character : MonoBehaviour,ISaveable
{
    [Header("事件监听")]
    public VoidEventSO newGameEvent;
    [Header("基本属性")]
    public float maxHealth;
    public float currentHealth;
    public float maxPower;
    public float currentPower;
    public float powerRecoverSpeed;
    [Header("受伤无敌")]
    public float invulnerableDuration;
    public float invulnerableCounter;
    public bool invulnerable;


    public UnityEvent<Character> OnHealthChange;
    public UnityEvent<Transform> OnTakeDamage;
    public UnityEvent OnDie;

    private void NewGame()
    {
        this.currentHealth = maxHealth;
        this.currentPower = maxPower;
        OnHealthChange?.Invoke(this);
    }
    private void Awake()
    {
        this.currentHealth = maxHealth;
    }
    private void OnEnable()
    {
        newGameEvent.OnEventRaised += NewGame;
        ISaveable saveable = this;
        saveable.RegisterSaveData();
    }
    private void OnDisable()
    {
        newGameEvent.OnEventRaised -= NewGame;
        ISaveable saveable = this;
        saveable.UnRegisterSaveData();
    }
    private void Update()
    {
        if(invulnerable)
        {
            invulnerableCounter -= Time.deltaTime; 
            if(invulnerableCounter <= 0 )
            {
                invulnerable = false;
            }
        }
        if(currentPower < maxPower)
        {
            currentPower += Time.deltaTime * powerRecoverSpeed;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Water"))
        {
            //死亡
            if(currentHealth > 0)
            {
                currentHealth = 0;
                OnHealthChange?.Invoke(this);
                OnDie?.Invoke();
            }    
        }
    }
    public void TackDamage(Attack attacker)
    {
        if(invulnerable)
            return;
        if(currentHealth - attacker.damage > 0)
        {
            currentHealth -= attacker.damage;
            TriggerInvulnerable();
            //执行受伤
            OnTakeDamage?.Invoke(attacker.transform);
        }
        else
        {
            //人物死亡
            currentHealth = 0;
            OnDie?.Invoke();
        }

        OnHealthChange?.Invoke(this);
    }

    public void TriggerInvulnerable()
    {
        if (!invulnerable) 
        {
            invulnerable = true;
            invulnerableCounter = invulnerableDuration;
        }
    }
    public void OnSlide(int cost)
    {
        currentPower -= cost;
        OnHealthChange?.Invoke(this);
    }
    public DataDefination GetDataID()
    {
        return GetComponent<DataDefination>();
    }
    public void GetSaveData(Data data)
    {
        if(data.characterPositionDic.ContainsKey(GetDataID().ID))
        {
            data.characterPositionDic[GetDataID().ID] = new SerializeVector3(transform.position);
            data.floatSaveData[GetDataID().ID + "health"] = this.currentHealth;
            data.floatSaveData[GetDataID().ID + "power"] = this.currentPower;
        }
        else
        {
            data.characterPositionDic.Add(GetDataID().ID, new SerializeVector3(transform.position));
            data.floatSaveData.Add(GetDataID().ID + "health", this.currentHealth);
            data.floatSaveData.Add(GetDataID().ID + "power", this.currentPower);
        }
    }

    public void LoadData(Data data)
    {
        if(data.characterPositionDic.ContainsKey(GetDataID().ID))
        {
            transform.position = data.characterPositionDic[GetDataID().ID].ToVector3();
            this.currentHealth = data.floatSaveData[GetDataID().ID + "health"];
            this.currentPower = data.floatSaveData[GetDataID().ID + "power"];

            //通知UI更新数据
            OnHealthChange?.Invoke(this);
        }
    }

   
}
