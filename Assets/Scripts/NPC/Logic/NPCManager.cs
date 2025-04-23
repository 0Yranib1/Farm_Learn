using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCManager : Singleton<NPCManager>
{
    public List<NPCPosition> npcPositionList;
    public SceneRouteDataList_SO sceneRouteData;
    private Dictionary<string,SceneRoute> sceneRouteDict=new Dictionary<string, SceneRoute>();


    protected override  void Awake()
    {
        base.Awake();
        InitSceneRouteDict();
    }

    private void OnEnable()
    {
        EventHandler.StartNewGameEvent += OnStartNewGameEvent;
    }

    private void OnDisable()
    {
        EventHandler.StartNewGameEvent -= OnStartNewGameEvent;
    }

    private void OnStartNewGameEvent(int obj)
    {
        foreach (var character in npcPositionList)
        {
            character.npc.position=character.position;
            character.npc.GetComponent<NPCMovement>().currentScene = character.startScene;
        }
    }

    private void InitSceneRouteDict()
    {
        if (sceneRouteData.sceneRouteList.Count > 0)
        {
            foreach (SceneRoute route in sceneRouteData.sceneRouteList)
            {
                var key = route.fromScentName + route.gotoSceneName;
                if (sceneRouteDict.ContainsKey(key))
                {
                    continue;
                }
                else
                {
                    sceneRouteDict.Add(key, route);
                }
            }
        }
    }
    
    /// <summary>
    /// 获得两个场景间路径
    /// </summary>
    /// <param name="fromScentName"></param>
    /// <param name="gotoSceneName"></param>
    /// <returns></returns>
    public SceneRoute GetSceneRoute(string fromScentName,string gotoSceneName)
    {
        return sceneRouteDict[fromScentName + gotoSceneName];
    }
    
}
