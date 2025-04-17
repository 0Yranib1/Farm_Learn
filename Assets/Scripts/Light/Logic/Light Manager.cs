using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightManager : MonoBehaviour
{
    private LightControl[] sceneLights;
    private LightShift currentLightShift;
    private Season currentSeason;

    private void OnEnable()
    {
        EventHandler.AfterSceneLoadEvent += OnAfterSceneLoadEvent;
    }
    private void OnDisable()
    {
        EventHandler.AfterSceneLoadEvent -= OnAfterSceneLoadEvent;
    }
    private void OnAfterSceneLoadEvent()
    {
        sceneLights = FindObjectsOfType<LightControl>();
        foreach (LightControl light in sceneLights)
        {
            //改变灯光方法
        }
    }
}
