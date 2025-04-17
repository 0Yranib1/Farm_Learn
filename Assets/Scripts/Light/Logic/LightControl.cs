using System;
using System.Collections;
using System.Collections.Generic;
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
}
