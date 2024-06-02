using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName ="Event/FloatEventSO")]
public class FloatEventSO : ScriptableObject
{
    public UnityAction<float> OnEventRaised;
    public void RaiseEvent(float f)
    {
        OnEventRaised?.Invoke(f);
    }
}
