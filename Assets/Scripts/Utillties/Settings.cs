using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Settings
{
    public const float itemFadeDuration = 0.35f;
    public  const float targetAlpha = 0.45f;
    //时间相关变量
    public const float secondThreshold = 0.1f;//数值越小时间越快
    public const int secondHold = 59;
    public const int minuteHold = 59;
    public const int hourHold = 23;
    public const int dayHold = 10;
    public const int seasonHold = 3;
    public const float fadeDuration = 1.5f;
    public const int reapAmount = 2;
    
    //NPC网格移动
    public const float gridCellSize = 1;
    public const float gridCellDiagonalSize = 1.41f;
    public const float pixelSize = 0.05f;
    //动画间隔
    public const float animationBreakTime = 5f;
    public const int maxGridSize = 9999;

    public static TimeSpan morningTime = new TimeSpan(5, 0, 0);
    public static TimeSpan nightTime = new TimeSpan(19, 0, 0);
    public const float lightChangeDuration = 25f;
    
    public static Vector3 playerStarPos= new Vector3(10.65f, -20.76f, 0);
    public static int playerStartMoney = 100;

}
