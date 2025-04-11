using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ScheduleDetails:IComparable<ScheduleDetails>
{
    public int hour, minute, day;
    public int priority;//优先级越小越先执行
    public Season season;
    public string targetScene;
    public Vector2Int targetGridPosition;
    public AnimationClip clipAtStop;
    public bool interactable;
    
    public int Time=> hour * 100 + minute;
    
    public ScheduleDetails(int hour, int minute, int day, int priority, Season season, string targetScene, Vector2Int targetGridPosition, AnimationClip clipAtStop, bool interactable)
    {
        this.hour = hour;
        this.minute = minute;
        this.day = day;
        this.priority = priority;
        this.season = season;
        this.targetScene = targetScene;
        this.targetGridPosition = targetGridPosition;
        this.clipAtStop = clipAtStop;
        this.interactable = interactable;
    }
    public int CompareTo(ScheduleDetails obj)
    {
        if (Time == obj.Time)
        {
            if (priority > obj.priority)
                return 1;
            else
            {
                return -1;
            }
        }else if (Time > obj.Time)
        {
            return 1;
        }else if (Time < obj.Time)
        {
            return -1;
        }

        return 0;
    }


}
