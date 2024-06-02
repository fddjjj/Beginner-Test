using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UImanager : MonoBehaviour
{
    public PlayerStateBar playerStateBar;
    [Header("事件监听")]
    public CharactorEventSO healthEvent;
    public SceneLoadEventSO unLoadSceneEvent;
    public VoidEventSO loadDataEvent;
    public VoidEventSO gameOverEvent;
    public VoidEventSO backToMenuEvent;
    public FloatEventSO syncVolumeEvent;
    public VoidEventSO closePauseEvent;

    [Header("广播")]
    public VoidEventSO pauseEvent;
    

    [Header("组件")]
    public GameObject gameOverPanel;
    public GameObject reStartButton;
    public GameObject mobileTouch;
    public Button settingsButton;
    public GameObject pausePanel;
    public Slider volumeSlider;

    private void Awake()
    {
        #if UNITY_STANDALONE
        mobileTouch.SetActive(false);
#endif

        settingsButton.onClick.AddListener(TogglePausePanel);
    }
    private void OnEnable()
    {
        healthEvent.OnEventRaised += OnHealthEvent;
        unLoadSceneEvent.LoadRequestEvent += OnUnLoadSceneEvent;
        loadDataEvent.OnEventRaised += OnLoadDataEvent;
        gameOverEvent.OnEventRaised += OnGameOverEvent;
        backToMenuEvent.OnEventRaised += OnLoadDataEvent;
        syncVolumeEvent.OnEventRaised += OnSyncVolumeEvent;
        closePauseEvent.OnEventRaised += ClosePauseEvent;
    }

    private void OnDisable()
    {
        healthEvent.OnEventRaised -= OnHealthEvent;
        unLoadSceneEvent.LoadRequestEvent -= OnUnLoadSceneEvent;
        loadDataEvent.OnEventRaised -= OnLoadDataEvent;
        gameOverEvent.OnEventRaised -= OnGameOverEvent;
        backToMenuEvent.OnEventRaised -= OnLoadDataEvent;
        syncVolumeEvent.OnEventRaised -= OnSyncVolumeEvent;
        closePauseEvent.OnEventRaised -= ClosePauseEvent;
    }

    private void ClosePauseEvent()
    {
        if (pausePanel.activeInHierarchy)
        {
            pausePanel.SetActive(false);
            Time.timeScale = 1.0f;
        }
    }

    private void OnSyncVolumeEvent(float amount)
    {
        volumeSlider.value = (amount + 80) / 100;
    }

    private void TogglePausePanel()
    {
        //pausePanel.SetActive(!pausePanel.activeInHierarchy);
        if(pausePanel.activeInHierarchy)
        {
            pausePanel.SetActive(false);
            Time.timeScale = 1.0f;
        }else
        {
            pauseEvent.RaiseEvent();
            pausePanel.SetActive(true);
            Time.timeScale = 0.0f;
        }    
    }

    private void OnUnLoadSceneEvent(GameSceneSO arg0, Vector3 arg1, bool arg2)
    {
        var isMenu = arg0.sceneType == SceneType.Menu;
        playerStateBar.gameObject.SetActive(!isMenu);
    }

    private void OnHealthEvent(Character character)
    {
        var persentage = character.currentHealth / character.maxHealth;
        playerStateBar.OnHealthChange(persentage);
        playerStateBar.OnPowerChange(character);
    }  
    private void OnLoadDataEvent()
    {
        gameOverPanel.SetActive(false);
    }
    private void OnGameOverEvent()
    {
        gameOverPanel.SetActive(true);
        EventSystem.current.SetSelectedGameObject(reStartButton);
    }

}
