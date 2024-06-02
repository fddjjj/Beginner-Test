using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportPoint : MonoBehaviour,IInteractable
{
    public Vector3 positionToGo;
    public GameSceneSO sceneToGo;
    public SceneLoadEventSO loadEventSO;
    private AudioDefination ad;
    private void Awake()
    {
        ad = GetComponent<AudioDefination>();
    }

    public void TriggerAction()
    {
        //Debug.Log("´«ËÍ£¡");

        loadEventSO.RaiseLoadRequestEvent(sceneToGo, positionToGo,true);
        ad.PlayAudioClip();
    }

}
