using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

public class SceneLoad : MonoBehaviour,ISaveable
{

    public Transform playerTrans;
    public Vector3 firstPosition;
    public Vector3 menuPosition;


    [Header("事件监听")]
    public SceneLoadEventSO loadEventSO;
    public VoidEventSO newGameEvent;
    public VoidEventSO backToMenuEvent;
    
    [Header("广播")]
    public VoidEventSO afterSceneLoadEventSO;
    public FadeEventSO fadeEvent;
    public SceneLoadEventSO unLoadEvent;

    [SerializeField]private GameSceneSO currentLoadScene;
    public GameSceneSO firstLoadScene;
    public GameSceneSO menuLoadScene;
    private GameSceneSO sceneToLoad;
    private Vector3 positionToGo;
    private bool fadeScene;
    private bool isLoading;
    public float fadeDuration;

    private void Awake()
    {
        //Addressables.LoadSceneAsync(firstLoadScene.sceneReference, UnityEngine.SceneManagement.LoadSceneMode.Additive);
        // currentLoadScene = firstLoadScene;
        //currentLoadScene.sceneReference.LoadSceneAsync(LoadSceneMode.Additive, true);
        loadEventSO.RaiseLoadRequestEvent(menuLoadScene, menuPosition, true);
    }
    private void OnEnable()
    {
        loadEventSO.LoadRequestEvent += OnLoadRequestEvent;
        newGameEvent.OnEventRaised += NewGame;
        backToMenuEvent.OnEventRaised += OnBackToMenuEvent;

        ISaveable saveable = this;
        saveable.RegisterSaveData();
    }
    private void OnDisable()
    {
        loadEventSO.LoadRequestEvent -= OnLoadRequestEvent;
        newGameEvent.OnEventRaised -= NewGame;
        backToMenuEvent.OnEventRaised -= OnBackToMenuEvent;

        ISaveable saveable = this;
        saveable.UnRegisterSaveData();
    }



    private void Start()
    {
        loadEventSO.RaiseLoadRequestEvent(menuLoadScene, menuPosition, true);
        //NewGame();
    }
    private void OnBackToMenuEvent()
    {
        sceneToLoad = menuLoadScene;
        loadEventSO.RaiseLoadRequestEvent(sceneToLoad, menuPosition, true);
    }   
    private void OnLoadRequestEvent(GameSceneSO locationToGo, Vector3 positionToGo, bool fadeScene)
    {
        if (isLoading)
            return;
        isLoading = true;
        sceneToLoad = locationToGo;
        this.positionToGo = positionToGo;
        this.fadeScene = fadeScene;
        // Debug.Log(sceneToLoad.sceneReference.SubObjectName);
        if (currentLoadScene != null)
        {
            StartCoroutine(UnLoadPreviousScene());
        }else
        {
            LoadNewScene();
        }
    }

    private IEnumerator UnLoadPreviousScene()
    {
        if(fadeScene)
        {
            //TODO:实现渐入渐出
            fadeEvent.FadeIn(fadeDuration);
        }
        yield return new WaitForSeconds(fadeDuration);
        //广播事件实现血条出现
        unLoadEvent.RaiseLoadRequestEvent(sceneToLoad, positionToGo, true);
        yield return  currentLoadScene.sceneReference.UnLoadScene();

       playerTrans.gameObject.SetActive(false);
        LoadNewScene();
    }
    private void LoadNewScene()
    {
        var loadingOption =  sceneToLoad.sceneReference.LoadSceneAsync(LoadSceneMode.Additive, true);
        loadingOption.Completed += OnLoadCompleted;
    }


    private void NewGame()
    {
        sceneToLoad = firstLoadScene;
        //OnLoadRequestEvent(sceneToLoad, firstPosition, true);
        loadEventSO.RaiseLoadRequestEvent(sceneToLoad, firstPosition, true);
    }
    /// <summary>
    /// 场景加载完成
    /// </summary>
    /// <param name="handle"></param>
    private void OnLoadCompleted(AsyncOperationHandle<SceneInstance> handle)
    {
        currentLoadScene = sceneToLoad;
        playerTrans.position = positionToGo;
        playerTrans.gameObject.SetActive(true);
        if (fadeScene)
        {
            fadeEvent.FadeOut(fadeDuration);
        }
        isLoading = false;
        if(currentLoadScene.sceneType != SceneType.Menu)
        {
            afterSceneLoadEventSO.RaiseEvent(); 
        }
        
    }

    public DataDefination GetDataID()
    {
        return GetComponent<DataDefination>();
    }

    public void GetSaveData(Data data)
    {
        data.SaveGameScene(this.currentLoadScene);
    }

    public void LoadData(Data data)
    {
        var playID = playerTrans.GetComponent<DataDefination>().ID;
        if (data.characterPositionDic.ContainsKey(playID))
        {
            positionToGo = data.characterPositionDic[playID].ToVector3();
            sceneToLoad = data.GetSaveGameScene();

            OnLoadRequestEvent(sceneToLoad,positionToGo, true);

        }
    }
}
