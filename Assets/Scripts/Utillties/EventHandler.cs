using System.Collections;
using System.Collections.Generic;
using System;
using MFarm.Dialogue;
using UnityEngine;

public static class EventHandler 
{
    
    public  static event System.Action<InventoryLocation,List<InventoryItem>> UpdateInventoryUI; 
    public  static void CallUpdateInventoryUI(InventoryLocation location,List<InventoryItem> itemList)
    {
        UpdateInventoryUI?.Invoke(location,itemList);
    }

    //在地图上生成物品
    public static event Action<int, Vector3> InstantiateItemInScene;
    public static void CallInstantiateItemInScene(int ID, Vector3 pos)
    {
        InstantiateItemInScene?.Invoke(ID,pos);
    }

    public static event Action<int, Vector3,ItemType> DropItemEvent;
    public static void CallDropItemEvent(int ID, Vector3 pos, ItemType itemType)
    {
        DropItemEvent?.Invoke(ID,pos,itemType);
    }
    
    public static event Action<ItemDetails, bool> ItemSelectEvent;
    public static void CallItemSelectEvent(ItemDetails itemDetails, bool isSelected)
    {
        ItemSelectEvent?.Invoke(itemDetails,isSelected);
    }

    public static event Action<int, int,int,Season> GameMinuteEvent;
    public static void CallGameMinuteEvent(int minute, int hour,int day, Season season)
    {
        GameMinuteEvent?.Invoke(minute,hour,day,season);
    }

    public static event Action<int,Season> GameDayEvent;
    public static void CallGameDayEvent(int day, Season season)
    {
        GameDayEvent?.Invoke(day,season);
    }
    
    public static event Action<int, int, int, int, Season> GameDateEvent;
    public static void CallGameDateEvent(int hour,int day, int month, int year, Season season)
    {
        GameDateEvent?.Invoke(hour,day,month,year,season);
    }

    public static event Action<string, Vector3> TransitionEvent;
    public static void CallTransitionEvent(string sceneName, Vector3 pos)
    {
        TransitionEvent?.Invoke(sceneName,pos);
    }

    public static event Action BeforeSceneUnloadEvent;
    public static void CallBeforeSceneUnloadEvent()
    {
        BeforeSceneUnloadEvent?.Invoke();
    }
    public static event Action AfterSceneLoadEvent;
    public static void CallAfterSceneLoadEvent()
    {
        AfterSceneLoadEvent?.Invoke();
    }

    public static event Action<Vector3> MoveToPosition;
    public static void CallMoveToPosition(Vector3 pos)
    {
        MoveToPosition?.Invoke(pos);
    }
    
    public static event Action<Vector3, ItemDetails> MouseClickedEvent;
    public static void CallMouseClickedEvent(Vector3 pos, ItemDetails itemDetails)
    {
        MouseClickedEvent?.Invoke(pos,itemDetails);
    }
    
    public static event Action<Vector3, ItemDetails> ExecuteActionAfterAnimation;
    public static void CallExecuteActionAfterAnimation(Vector3 pos, ItemDetails itemDetails)
    {
        ExecuteActionAfterAnimation?.Invoke(pos,itemDetails);
    }

    public static Action<int, TileDetails> PlantSeedEvent;
    public static void CallPlantSeedEvent(int seedItemID, TileDetails tileDetails)
    {
        PlantSeedEvent?.Invoke(seedItemID,tileDetails);
    }
    
    public static event Action<int> HarvestAtPlayerPosition;
    public static void CallHarvestAtPlayerPosition(int itemID)
    {
        HarvestAtPlayerPosition?.Invoke(itemID);
    }

    public static event Action RefreshCurrentMap;
    public static void CallRefreshCurrentMap()
    {
        RefreshCurrentMap?.Invoke();
    }

    public static Action<ParticaleEffectType, Vector3> ParticleEffectEvent;
    public static void CallParticleEffectEvent(ParticaleEffectType type, Vector3 pos)
    {
        ParticleEffectEvent?.Invoke(type,pos);
    }

    public static event Action GenerateCropEvent;
    public static void CallGenerateCropEvent()
    {
        GenerateCropEvent?.Invoke();
    }

    public static event Action<DialoguePiece> ShowDialogueEvent;

    public static void CallShowDialogueEvent(DialoguePiece piece)
    {
        ShowDialogueEvent?.Invoke(piece);
    }
    
    //商店开启
    public static event Action<SlotType, InventoryBag_SO> BaseBagOpenEvent;
    public static void CallBaseBagOpenEvent(SlotType slotType, InventoryBag_SO bag_SO)
    {
        BaseBagOpenEvent?.Invoke(slotType,bag_SO);
    }
    
    public static event Action<SlotType, InventoryBag_SO> BaseBagCloseEvent;
    public static void CallBaseBagCloseEvent(SlotType slotType, InventoryBag_SO bag_SO)
    {
        BaseBagCloseEvent?.Invoke(slotType,bag_SO);
    }

    public static event Action<GameState> UpdateGameStateEvent;
    public static void CallUpdateGameStateEvent(GameState state)
    {
        UpdateGameStateEvent?.Invoke(state);
    }

    public static event Action<ItemDetails, bool> ShowTradeUI;
    public static void CallShowTradeUI(ItemDetails itemDetails, bool isSell)
    {
        ShowTradeUI?.Invoke(itemDetails,isSell);
    }
    
    //建造
    public static event Action<int,Vector3 > BuildFurnitureEvent;
    public static void CallBuildFurnitureEvent(int ID, Vector3 mousePos)
    {
        BuildFurnitureEvent?.Invoke(ID,mousePos);
    }

}
