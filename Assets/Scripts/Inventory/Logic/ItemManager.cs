using System;
using System.Collections;
using System.Collections.Generic;
using MFarm.Inventory;
using MFarm.Save;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ItemManager : MonoBehaviour,ISaveable
{
    public Item itemPrefab;

    public Item bouncePrefab;

    private Transform playerTransform => FindObjectOfType<Player>().transform;
    
     [SerializeField]private Transform itemParent;
    
     //记录场景
     private Dictionary<string, List<SceneItem>> sceneItemDict = new Dictionary<string, List<SceneItem>>();
     //家具
     private Dictionary<string, List<SceneFurniture>> sceneFurnitureDict= new Dictionary<string, List<SceneFurniture>>();
     
     public string GUID => GetComponent<DataGUID>().guid;
     public GameSaveData generateSaveData()
     {
         
         GetAllSceneItems();
         GetAllSceneFurniture();
         
         GameSaveData saveData = new GameSaveData();
         
         saveData.sceneItemDict = sceneItemDict;
         saveData.sceneFurnitureDict = sceneFurnitureDict;
         
         return saveData;
     }

     public void RestoreData(GameSaveData saveData)
     {
         this.sceneItemDict= saveData.sceneItemDict;
         this.sceneFurnitureDict= saveData.sceneFurnitureDict;
         
         RecreateAllItems();
         RebuildFurniture();
     }

     private void Start()
     {
         ISaveable saveable = this;
         saveable.RegisterSaveable();
     }

     private void OnEnable()
    {
        EventHandler.InstantiateItemInScene += OnInstantiateItemInScene;
        EventHandler.BeforeSceneUnloadEvent += OnBeforeSceneUnloadEvent;
        EventHandler.AfterSceneLoadEvent += OnAfterSceneLoadedEvent;
        EventHandler.DropItemEvent += OnDropItemEvent;
        EventHandler.BuildFurnitureEvent += OnBuildFurnitureEvent;
    }
    

    private void OnDisable()
    {
        EventHandler.InstantiateItemInScene -= OnInstantiateItemInScene;
        EventHandler.BeforeSceneUnloadEvent -= OnBeforeSceneUnloadEvent;
        EventHandler.AfterSceneLoadEvent -= OnAfterSceneLoadedEvent;
        EventHandler.DropItemEvent -= OnDropItemEvent;
        EventHandler.BuildFurnitureEvent -= OnBuildFurnitureEvent;
    }

    private void OnBuildFurnitureEvent(int ID, Vector3 mousePos)
    {
        BluePrintDetails bluePrint = InventoryManager.Instance.bluePrintData.GetBluePrintDetails(ID);
        var buildItem = Instantiate(bluePrint.buildPrefab, mousePos, Quaternion.identity, itemParent);
        if (buildItem.GetComponent<Box>())
        {
            buildItem.GetComponent<Box>().index = InventoryManager.Instance.BoxDataAmount;
            buildItem.GetComponent<Box>().InitBox(buildItem.GetComponent<Box>().index);
        }
    }

    private void OnBeforeSceneUnloadEvent()
    {
        GetAllSceneItems();
        GetAllSceneFurniture();
    }
    public void OnAfterSceneLoadedEvent()
    {
        itemParent = GameObject.FindWithTag("ItemParent").transform;
        RecreateAllItems();
        RebuildFurniture();
    }
    /// <summary>
    /// 在指定位置生成物品
    /// </summary>
    /// <param name="ID"></param>
    /// <param name="pos"></param>
    public void OnInstantiateItemInScene(int ID, Vector3 pos)
    {
        Item item = Instantiate(bouncePrefab, pos, Quaternion.identity, itemParent);
        item.itemID=ID;
        item.GetComponent<ItemBounce>().InitBounceItem(pos, Vector3.up);
    }

    private void OnDropItemEvent(int ID, Vector3 mousePos,ItemType itemType)
    {
        if (itemType == ItemType.Seed)
        {
            return;
        }
        //扔东西的效果
        Item item = Instantiate(bouncePrefab, playerTransform.position, Quaternion.identity, itemParent);
        item.itemID = ID;
        var dir = (mousePos - playerTransform.position).normalized;
        item.GetComponent<ItemBounce>().InitBounceItem(mousePos, dir);
    }
    
    /// <summary>
    /// 获得当场景所有物品
    /// </summary>
    private void GetAllSceneItems()
    {
        List<SceneItem> currentScentItems = new List<SceneItem>();
        foreach (var item in FindObjectsOfType<Item>())
        {
            SceneItem sceneItem = new SceneItem
            {
                itemID = item.itemID,
                position = new SerializableVector3(item.transform.position)
            };
            currentScentItems.Add(sceneItem);
        }

        if (sceneItemDict.ContainsKey(SceneManager.GetActiveScene().name))
        {
            //找到数据更新
            sceneItemDict[SceneManager.GetActiveScene().name] = currentScentItems;
        }
        else
        {
            //如果是新场景添加数据
            sceneItemDict.Add(SceneManager.GetActiveScene().name, currentScentItems);
        }
    }

    /// <summary>
    /// 刷新场景内物品
    /// </summary>
    private void RecreateAllItems()
    {
        List<SceneItem> currentSceneItems = new List<SceneItem>();
        if (sceneItemDict.TryGetValue(SceneManager.GetActiveScene().name, out currentSceneItems))
        {
            if (currentSceneItems != null)
            {
                //清空场景内物品
                foreach (var item in FindObjectsOfType<Item>())
                {
                    Destroy(item.gameObject);
                }
                //重新创建列表物品
                foreach (var item in currentSceneItems)
                {
                    Item newItem = Instantiate(itemPrefab, item.position.ToVector3(), Quaternion.identity, itemParent);
                    newItem.Init(item.itemID);
                }
            }
        }
    }

    /// <summary>
    /// 获得当场景所有家具
    /// </summary>
    private void GetAllSceneFurniture()
    {
        List<SceneFurniture> currentSceneFurniture = new List<SceneFurniture>();
        foreach (var item in FindObjectsOfType<Furniture>())
        {
            SceneFurniture sceneFurniture = new SceneFurniture
            {
                itemID = item.itemID,
                position = new SerializableVector3(item.transform.position)
            };
            if (item.GetComponent<Box>())
            {
                sceneFurniture.boxIndex= item.GetComponent<Box>().index;
            }
            currentSceneFurniture.Add(sceneFurniture);
        }

        if (sceneFurnitureDict.ContainsKey(SceneManager.GetActiveScene().name))
        {
            //找到数据更新
            sceneFurnitureDict[SceneManager.GetActiveScene().name] = currentSceneFurniture;
        }
        else
        {
            //如果是新场景添加数据
            sceneFurnitureDict.Add(SceneManager.GetActiveScene().name, currentSceneFurniture);
        }
    }

    /// <summary>
    /// 重建当前场景家具
    /// </summary>
    private void RebuildFurniture()
    {
        List<SceneFurniture> currentSceneFurniture = new List<SceneFurniture>();

        if (sceneFurnitureDict.TryGetValue(SceneManager.GetActiveScene().name, out currentSceneFurniture))
        {
            if (currentSceneFurniture != null)
            {
                foreach (SceneFurniture sceneFurniture in currentSceneFurniture)
                {
                    BluePrintDetails bluePrint = InventoryManager.Instance.bluePrintData.GetBluePrintDetails(sceneFurniture.itemID);
                    var buildItem = Instantiate(bluePrint.buildPrefab, sceneFurniture.position.ToVector3(), Quaternion.identity, itemParent);
                    if (buildItem.GetComponent<Box>())
                    {
                        buildItem.GetComponent<Box>().InitBox(sceneFurniture.boxIndex);
                    }
                }
            }
        }
    }
    
}
