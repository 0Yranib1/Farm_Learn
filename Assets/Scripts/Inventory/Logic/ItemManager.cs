using System;
using System.Collections;
using System.Collections.Generic;
using MFarm.Inventory;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ItemManager : MonoBehaviour
{
    public Item itemPrefab;

    public Item bouncePrefab;

    private Transform playerTransform => FindObjectOfType<Player>().transform;
    
     [SerializeField]private Transform itemParent;
    
     //记录场景
     private Dictionary<string, List<SceneItem>> sceneItemDict = new Dictionary<string, List<SceneItem>>();
     
    private void OnEnable()
    {
        EventHandler.InstantiateItemInScene += OnInstantiateItemInScene;
        EventHandler.BeforeSceneUnloadEvent += OnBeforeSceneUnloadEvent;
        EventHandler.AfterSceneLoadEvent += OnAfterSceneLoadedEvent;
        EventHandler.DropItemEvent += OnDropItemEvent;
    }
    

    private void OnDisable()
    {
        EventHandler.InstantiateItemInScene -= OnInstantiateItemInScene;
        EventHandler.BeforeSceneUnloadEvent -= OnBeforeSceneUnloadEvent;
        EventHandler.AfterSceneLoadEvent -= OnAfterSceneLoadedEvent;
        EventHandler.DropItemEvent -= OnDropItemEvent;
    }
    private void OnBeforeSceneUnloadEvent()
    {
        GetAllSceneItems();
    }
    public void OnAfterSceneLoadedEvent()
    {
        itemParent = GameObject.FindWithTag("ItemParent").transform;
        RecreateAllItems();
    }
    /// <summary>
    /// 在指定位置生成物品
    /// </summary>
    /// <param name="ID"></param>
    /// <param name="pos"></param>
    public void OnInstantiateItemInScene(int ID, Vector3 pos)
    {
        Item item = Instantiate(itemPrefab, pos, Quaternion.identity, itemParent);
        item.itemID=ID;
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
}
