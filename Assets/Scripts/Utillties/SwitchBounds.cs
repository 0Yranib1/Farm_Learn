using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class SwitchBounds : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnEnable()
    {
        EventHandler.AfterSceneLoadEvent+=SwitchConfinerShape;
    }
    private void OnDisable()
    {
        
        EventHandler.AfterSceneLoadEvent-=SwitchConfinerShape;
    }


    private void SwitchConfinerShape()
    {
        PolygonCollider2D confinerShape=GameObject.FindGameObjectWithTag("BoundsConfiner").GetComponent<PolygonCollider2D>();
        CinemachineConfiner confiner=GetComponent<CinemachineConfiner>();
        confiner.m_BoundingShape2D=confinerShape;
        //每次runtime切换confiner的bounds时应调用一下代码清除缓存
        confiner.InvalidatePathCache();
    }
}
