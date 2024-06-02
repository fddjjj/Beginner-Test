using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStateBar : MonoBehaviour
{
    public Character currentCharacter;
    public Image healthImage;
    public Image healthDelayImage;
    public Image powerImage;

    private bool isRecovering;

    private void Update()
    {
        if (healthDelayImage.fillAmount > healthImage.fillAmount)
        {
            healthDelayImage.fillAmount -= Time.deltaTime * (float)0.1;
        }
        if(isRecovering)
        {
            float persantage = currentCharacter.currentPower / currentCharacter.maxPower;
            powerImage.fillAmount = persantage;
            if(persantage >= 1)
            {
                isRecovering = false;
                return;

            }
        }
    }
    ///   <summary>
    ///    ����Health�ı���ٷֱ� 
    ///    </summary>
    ///     <param name="persantage">�ٷֱȣ�Current/Max</param> 
    public void OnHealthChange(float persantage)
    {
        healthImage.fillAmount = persantage;
    }
    public void OnPowerChange(Character character)
    {
        isRecovering = true;
        currentCharacter = character;

    }
}
