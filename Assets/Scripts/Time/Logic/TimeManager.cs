using System;
using System.Collections;
using System.Collections.Generic;
using MFarm.Save;
using UnityEditor;
using UnityEngine;

public class TimeManager : Singleton<TimeManager>, ISaveable
{
    [SerializeField] int gameSecond,gameMinute,gameHour,gameDay,gameMonth,gameYear;
    [SerializeField] private Season gameSeason = Season.春天;
    [SerializeField] private int monthInSeason = 3;
    public bool gameClockPause;

    private float tikTime;

    public TimeSpan GameTime => new TimeSpan(gameHour, gameMinute, gameSecond);
    
    //灯光时间差
    private float timeDifference;
    
    public string GUID => GetComponent<DataGUID>().guid;
    public GameSaveData generateSaveData()
    {
        GameSaveData saveData = new GameSaveData();
        saveData.timeDict = new Dictionary<string, int>();
        saveData.timeDict.Add("gameSecond", gameSecond);
        saveData.timeDict.Add("gameMinute", gameMinute);
        saveData.timeDict.Add("gameHour", gameHour);
        saveData.timeDict.Add("gameDay", gameDay);
        saveData.timeDict.Add("gameMonth", gameMonth);
        saveData.timeDict.Add("gameSeason", (int)gameSeason);
        saveData.timeDict.Add("gameYear", gameYear);
        
        return saveData;
    }

    public void RestoreData(GameSaveData saveData)
    {
        gameYear= saveData.timeDict["gameYear"];
        gameMonth= saveData.timeDict["gameMonth"];
        gameDay= saveData.timeDict["gameDay"];
        gameHour= saveData.timeDict["gameHour"];
        gameMinute= saveData.timeDict["gameMinute"];
        gameSecond= saveData.timeDict["gameSecond"];
        gameSeason= (Season)saveData.timeDict["gameSeason"];
    }

    protected override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        // EventHandler.CallGameDateEvent(gameHour, gameDay, gameMonth, gameYear, gameSeason);
        // EventHandler.CallGameMinuteEvent(gameMinute, gameHour,gameDay, gameSeason);
        // //灯光切换
        // EventHandler.CallLightShiftChangeEvent(gameSeason, getCurrentLightShift(), timeDifference);
        
        ISaveable saveable = this;
        saveable.RegisterSaveable();
        gameClockPause = true;
    }

    private void OnEnable()
    {
        EventHandler.BeforeSceneUnloadEvent+=OnBeforeSceneUnloadEvent;
        EventHandler.AfterSceneLoadEvent += OnAfterSceneLoadEvent;
        EventHandler.UpdateGameStateEvent += OnUpdateGameStateEvent;
        EventHandler.StartNewGameEvent += OnStartNewGameEvent;
        EventHandler.EndGameEvent += OnEndGameEvent;
    }

    private void OnDisable()
    {
        EventHandler.BeforeSceneUnloadEvent -= OnBeforeSceneUnloadEvent;
        EventHandler.AfterSceneLoadEvent -= OnAfterSceneLoadEvent;
        EventHandler.UpdateGameStateEvent -= OnUpdateGameStateEvent;
        EventHandler.StartNewGameEvent -= OnStartNewGameEvent;
        EventHandler.EndGameEvent -= OnEndGameEvent;
    }

    private void OnEndGameEvent()
    {
        gameClockPause = true;
    }

    private void OnStartNewGameEvent(int obj)
    {
        NewGameTime();
        gameClockPause = false;
    }

    void OnUpdateGameStateEvent(GameState state)
    {
        gameClockPause = state == GameState.Pause;
    }

    void OnBeforeSceneUnloadEvent()
    {
        gameClockPause = true;
    }
    
    void OnAfterSceneLoadEvent()
    {
        gameClockPause = false;
    }
    
    private void Update()
    {
        if (!gameClockPause)
        {
            tikTime += Time.deltaTime;
            if (tikTime >= Settings.secondThreshold)
            {
                tikTime -= Settings.secondThreshold;
                UpdateGameTime();
            }
        }

        if (Input.GetKey(KeyCode.T))
        {
            for (int i = 0; i < 60; i++)
            {
                UpdateGameTime();
            }
        }
        if (Input.GetKeyDown(KeyCode.G))
        {
            gameDay++;
            EventHandler.CallGameDayEvent(gameDay, gameSeason);
            EventHandler.CallGameDateEvent(gameHour, gameDay, gameMonth, gameYear, gameSeason);
        }
    }

    private void NewGameTime()
    {
        gameSecond = 0;
        gameMinute = 0;
        gameHour = 7;
        gameDay = 1;
        gameMonth = 1;
        gameYear = 1;
        gameSeason = Season.春天;
        gameClockPause = false;
    }
    
    //时间更新
    private void UpdateGameTime()
    {
        gameSecond++;
        if (gameSecond > Settings.secondHold)
        {
            gameSecond = 0;
            gameMinute++;
            if (gameMinute > Settings.minuteHold)
            {
                gameMinute = 0;
                gameHour++;
                if (gameHour > Settings.hourHold)
                {
                    gameHour = 0;
                    gameDay++;
                    if (gameDay > Settings.dayHold)
                    {
                        gameDay = 1;
                        gameMonth++;

                        if (gameMonth > 12)
                        {
                            gameMonth = 1;
                        }

                        monthInSeason--;
                        if (monthInSeason == 0)
                        {
                            monthInSeason = 3;
                            int seasonNumber = (int)gameSeason;
                            seasonNumber++;
                            if (seasonNumber > Settings.seasonHold)
                            {
                                seasonNumber = 0;
                                gameYear++;
                            }
                            gameSeason=(Season)seasonNumber;
                        }
                        //刷新地图和和农作物生长
                        EventHandler.CallGameDayEvent(gameDay, gameSeason);
                    }
                }
                EventHandler.CallGameDateEvent(gameHour, gameDay, gameMonth, gameYear, gameSeason);
            }
            EventHandler.CallGameMinuteEvent(gameMinute, gameHour,gameDay,gameSeason);
            //切换灯光
            EventHandler.CallLightShiftChangeEvent(gameSeason, getCurrentLightShift(), timeDifference);
        }
    }

    /// <summary>
    /// 返回lightshift计算时间差
    /// </summary>
    /// <returns></returns>
    private LightShift getCurrentLightShift()
    {
        if (GameTime >= Settings.morningTime && GameTime < Settings.nightTime)
        {
            timeDifference = (float)(GameTime - Settings.morningTime).TotalMinutes;
            return LightShift.Morning;
        }

        if (GameTime < Settings.morningTime || GameTime >= Settings.nightTime)
        {
            timeDifference = Mathf.Abs((float)(GameTime - Settings.nightTime).TotalMinutes);
            return LightShift.Night;
        }

        return LightShift.Morning;
    }
}
