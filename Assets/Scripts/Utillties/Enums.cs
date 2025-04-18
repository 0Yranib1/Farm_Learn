using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType 
{
    Seed,Commodity,Furniture,
    HoeTool,ChopTool,BreakTool,ReapTool,WaterTool,CollectTool,
    ReapableScenery
}

public enum SlotType
{
    Bag,Box,Shop
}

public enum InventoryLocation
{
    Player,Shop,Box
}

public enum PartType
{
    None,Carry,Hoe,Break,Water,Collect,Chop,Reap
}

public enum PartName
{
    Body,Hair,Arm,Tool
}

public enum Season
{
    春天,夏天,秋天,冬天
}

public enum GridType
{
    Diggable,DropItem,PlaceFurniture,NPCObstacle
}

public enum ParticaleEffectType
{
    None,LeavesFalling01,LeavesFalling02,Rock,ReapableScenery
}

public enum GameState
{
    Gameplay,Pause
}

public enum LightShift
{
    Morning,Night
}

public enum SoundName
{
    none,FootStepSoft,FootStepHard,
    Axe,Pickaxe,Hoe,Reap,Water,Basket,Chop,
    PickUp,Plant,TreeFalling,Rustle,
    AmbientCountryside1,AmbientCountrySide2,MusicCalm1,MusicCalm2,MusicCalm3,MusicCalm4,MusicCalm5,MusicCalm6,AmbientIndoor1
}