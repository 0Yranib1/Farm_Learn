using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightManager : MonoBehaviour
{
    private LightControl[] sceneLights;
    private LightShift currentLightShift;
    private Season currentSeason;
    private float timeDifference;

    private void OnEnable()
    {
        EventHandler.AfterSceneLoadEvent += OnAfterSceneLoadEvent;
        EventHandler.LightShiftChangeEvent += OnLightShiftChangeEvent;
    }
    private void OnDisable()
    {
        EventHandler.AfterSceneLoadEvent -= OnAfterSceneLoadEvent;
        EventHandler.LightShiftChangeEvent -= OnLightShiftChangeEvent;
    }

    private void OnLightShiftChangeEvent(Season season, LightShift lightShift, float timeDifference)
    {
        currentSeason=season;
        this.timeDifference = timeDifference;
        if (currentLightShift != lightShift)
        {
            currentLightShift = lightShift;
            foreach (LightControl light in sceneLights)
            {
                //改变灯光方法
                light.ChangeLight(currentSeason, currentLightShift, timeDifference);
            }
        }
    }

    private void OnAfterSceneLoadEvent()
    {
        sceneLights = FindObjectsOfType<LightControl>();
        foreach (LightControl light in sceneLights)
        {
            //改变灯光方法
            light.ChangeLight(currentSeason, currentLightShift, timeDifference);
        }
    }
}
