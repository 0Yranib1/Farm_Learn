using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightControl : MonoBehaviour
{
    public LightPattenList_SO lightData;
    public Light2D currentLight;
    private LightDetails currentLightDetails;

    private void Awake()
    {
        currentLight = GetComponent<Light2D>();
    }
    //实际切换灯光
    public void ChangeLight(Season season, LightShift lightShift, float timeDifference)
    {
        currentLightDetails = lightData.GetLightDetails(season, lightShift);

        if (timeDifference < Settings.lightChangeDuration)
        {
            var colorOffset = (currentLightDetails.lightColor - currentLight.color) / Settings.lightChangeDuration *
                              timeDifference;
            currentLight.color += colorOffset;
            DOTween.To(() => currentLight.color, x => currentLight.color = x, currentLightDetails.lightColor,
                Settings.lightChangeDuration - timeDifference);
            DOTween.To(() => currentLight.intensity, x => currentLight.intensity = x, currentLightDetails.lightAmount,
                Settings.lightChangeDuration - timeDifference);
        }

        if (timeDifference >= Settings.lightChangeDuration)
        {
            currentLight.color = currentLightDetails.lightColor;
            currentLight.intensity = currentLightDetails.lightAmount;
        }
    }
}
