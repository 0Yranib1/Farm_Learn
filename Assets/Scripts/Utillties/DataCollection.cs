using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[System.Serializable]

public class ItemDetails
{
    public int itemID;
    public  string itemName;
    public ItemType ItemType;
    public Sprite itemIcon;
    public Sprite itemOnWorldSprite;
    public string itemDescription;
    public int itemUseRadius;
    public bool canDropped;
    public bool canPickedUp;
    public bool canCarried;
    public int itemPrice;
    [Range(0,1)]
    public float sellPercentage;
}
[System.Serializable]
public struct InventoryItem
{
    public int itemID;
    public int itemAmount;
}

[System.Serializable]
public class AnimatorType
{
    public PartType partType;
    public PartName partName;
    public AnimatorOverrideController overrideController;
}

[System.Serializable]
public class SerializableVector3
{
    public float x;
    public float y;
    public float z;
    public SerializableVector3(Vector3 vector3)
    {
        x = vector3.x;
        y = vector3.y;
        z = vector3.z;
    }
    
    public Vector3 ToVector3()
    {
        return new Vector3(x, y, z);
    }
    public Vector2Int ToVector2Int()
    {
        return new Vector2Int((int)x, (int)y);
    }
}

[System.Serializable]
public class SceneItem
{
    public int itemID;
    public SerializableVector3 position;
}

[Serializable]
public class SceneFurniture
{
    public int itemID;
    public SerializableVector3 position;
    public int boxIndex;
}
[System.Serializable]
public class TileProperty
{
    public Vector2Int tileCoordinate;
    public GridType gridType;
    public bool boolTypeValue;
}
[System.Serializable]
public class TileDetails
{
    public int gridX, gridY;
    public bool canDig;
    public bool canDropItem;
    public bool canPlaceFurniture;
    public bool isNPCObstacle;
    public int daysSinceDug=-1;
    public int daysSinceWatered=-1;
    public int seedItemID = -1;
    public int growthDays = -1;
    public int daysSinceLastHarvest = -1;
}

[Serializable]
public class NPCPosition
{
    public Transform npc;
    [FormerlySerializedAs("startScent")] public string startScene;
    public Vector3 position;

}
[Serializable]
public class SceneRoute
{
    public string fromScentName;
    public string gotoSceneName;
    public List<ScenePath> scenePathList;
}

//场景路径
[Serializable]
public class ScenePath
{
    public string sceneName;
    public Vector2Int fromGridCell;
    public Vector2Int gotoGridCell;
}