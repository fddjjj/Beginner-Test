using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Newtonsoft.Json;
using System.IO;


[DefaultExecutionOrder(-100)]
public class DataManager : MonoBehaviour
{
    [Header("¼àÌý")]
    public VoidEventSO saveDataEvent;
    public VoidEventSO loadGameDataEvent;

    public static DataManager instance;
    private List<ISaveable> saveableList = new List<ISaveable>();
    private Data saveData;
    private string jsonFolder;
    
    private void Awake()
    {
        if(instance == null)
            instance = this;
        else
            Destroy(instance);
        saveData = new Data();
        jsonFolder = Application.persistentDataPath + "/Save Data/";


        ReadSaveData();
    }

    private void OnEnable()
    {
        saveDataEvent.OnEventRaised += Save;
        loadGameDataEvent.OnEventRaised += Load;
    }

    private void OnDisable()
    {
        saveDataEvent.OnEventRaised -= Save;
        loadGameDataEvent.OnEventRaised -= Load;
    }

    private void Update()
    {
        if(Keyboard.current.lKey.wasPressedThisFrame)
        {
            Load();
        }
    }
    public void RegisterSaveData(ISaveable saveable)
    {
        if (!saveableList.Contains(saveable))
        {
            saveableList.Add(saveable);
        }
    }

    public void UnRegisterSaveData(ISaveable saveable)
    {
        saveableList.Remove(saveable);
    }
    public void Save()
    {
        foreach(var saveable in saveableList)
        {
            saveable.GetSaveData(saveData);
        }

        var resultPath = jsonFolder + "data.sav";

        var jsonData = JsonConvert.SerializeObject(saveData);
        if(!File.Exists(resultPath))
        {
            Directory.CreateDirectory(jsonFolder);
        }

        File.WriteAllText(resultPath, jsonData);

        //foreach(var Item in saveData.characterPositionDic)
        //{
        //    Debug.Log(Item.Key + "   " + Item.Value);
        //}
    }

    public void Load()
    {
        foreach (var saveable in saveableList)
        {
            saveable.LoadData(saveData);
        }
    }

    private void ReadSaveData()
    {
        var resultPath = jsonFolder + "data.sav";

        if (File.Exists(resultPath))
        {
            var stringData = File.ReadAllText(resultPath);

            var jsonData = JsonConvert.DeserializeObject<Data>(stringData);
            saveData = jsonData; 
        }
    }
}
