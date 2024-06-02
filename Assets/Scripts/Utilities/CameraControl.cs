using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using TMPro;
using System;

public class CameraControl : MonoBehaviour
{
    [Header("ÊÂ¼þ¼àÌý")]
    public VoidEventSO afterSceneLoadEventSO;

    private CinemachineConfiner2D confiner2D;
    public CinemachineImpulseSource impulseSource;
    public VoidEventSO cameraSharkEvent;
    private void Awake()
    {
        confiner2D = GetComponent<CinemachineConfiner2D>();

    }
//private void Start()
  //  {
    //    GetNewCameraBounds();
   // }
    private void OnEnable()
    {
        cameraSharkEvent.OnEventRaised += OnCameraShakeEvent;
        afterSceneLoadEventSO.OnEventRaised += OnAfterSceneLoadEventSO;
    }

    
    private void OnDisable()
    {
        cameraSharkEvent.OnEventRaised -= OnCameraShakeEvent;
        afterSceneLoadEventSO.OnEventRaised -= OnAfterSceneLoadEventSO;
    }

    private void OnAfterSceneLoadEventSO()
    {
        GetNewCameraBounds();
    }

    private void OnCameraShakeEvent()
    {
        impulseSource.GenerateImpulse();
    }
    private void GetNewCameraBounds()
    {
        var obj = GameObject.FindGameObjectWithTag("Bounds");
        if(obj == null)
            return;
        confiner2D.m_BoundingShape2D = obj.GetComponent<Collider2D>(); 
        confiner2D.InvalidateCache();
    }
}
