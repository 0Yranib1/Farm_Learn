using System;
using System.Collections;
using System.Collections.Generic;
using MFarm.CropPlant;
using MFarm.Save;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

namespace MFarm.Map
{
    public class GridMapManager : Singleton<GridMapManager>, ISaveable
    {
        
        [Header("种地瓦片")] 
        public RuleTile digTile;
        public RuleTile waterTile;
        private Tilemap digTilemap;
        private Tilemap waterTilemap;
        
        [Header("地图信息")] 
        public List<MapData_SO> MapDataList;

        private Season currentSeason;
        //场景名称+坐标和对应瓦片信息
        private Dictionary<string, TileDetails> tileDetailsDict = new Dictionary<string, TileDetails>();

        //场景是否第一次加载
        private Dictionary<string, bool> firstLoadDict = new Dictionary<string, bool>();

        private Grid currentGrid;
        
        //杂草列表
        private List<ReapItem> itemsInRadius;
        
        public string GUID => GetComponent<DataGUID>().guid;
        public GameSaveData generateSaveData()
        {
            GameSaveData saveData= new GameSaveData();
            saveData.tileDetailsDict = tileDetailsDict;
            saveData.firstLoadDict = firstLoadDict;
            return saveData;
        }

        public void RestoreData(GameSaveData saveData)
        {
            this.tileDetailsDict= saveData.tileDetailsDict;
            this.firstLoadDict= saveData.firstLoadDict;
        }

        private void OnEnable()
        {
            EventHandler.ExecuteActionAfterAnimation+=OnExecuteActionAfterAnimation;
            EventHandler.AfterSceneLoadEvent += OnAfterSceneLoadedEvent;
            EventHandler.GameDayEvent += OnGameDayEvent;
            EventHandler.RefreshCurrentMap += RefreshMap;
        }

        private void OnDisable()
        {
            EventHandler.ExecuteActionAfterAnimation -= OnExecuteActionAfterAnimation;
            EventHandler.AfterSceneLoadEvent -= OnAfterSceneLoadedEvent;
            EventHandler.GameDayEvent -= OnGameDayEvent;
            EventHandler.RefreshCurrentMap -= RefreshMap;
        }

        
        private void Start()
        {
            foreach (var mapData in MapDataList)
            {
                firstLoadDict.Add(mapData.sceneName, true);
                InitTileDetailsDict(mapData);
            }
            
            ISaveable saveable = this;
            saveable.RegisterSaveable();
        }

        private void OnAfterSceneLoadedEvent()
        {
            currentGrid = FindObjectOfType<Grid>();
            digTilemap = GameObject.FindWithTag("Dig").GetComponent<Tilemap>();
            waterTilemap = GameObject.FindWithTag("Water").GetComponent<Tilemap>();
            
            // DisplayMap(SceneManager.GetActiveScene().name);

            if (firstLoadDict[SceneManager.GetActiveScene().name])
            {
                //预先生成农作物
                EventHandler.CallGenerateCropEvent();
                firstLoadDict[SceneManager.GetActiveScene().name] = false;
            }
            
            RefreshMap();
        }
    /// <summary>
    /// 每天运行一次
    /// </summary>
    /// <param name="day"></param>
    /// <param name="season"></param>
        private void OnGameDayEvent(int day,Season season)
        {
            currentSeason = season;
            foreach (var tile in tileDetailsDict)
            {
                if (tile.Value.daysSinceWatered > -1)
                {
                    tile.Value.daysSinceWatered=-1;
                }
                if (tile.Value.daysSinceDug > -1)
                {
                    tile.Value.daysSinceDug ++;
                }
                //超期消除挖坑
                if (tile.Value.daysSinceDug > 5 && tile.Value.seedItemID==-1)
                {
                    tile.Value.daysSinceDug = -1;
                    tile.Value.canDig = true;
                    tile.Value.canDropItem = true;
                    tile.Value.growthDays = -1;
                }

                if (tile.Value.seedItemID != -1)
                {
                    tile.Value.growthDays++;
                }
            }
            RefreshMap();
        }
        
        private void InitTileDetailsDict(MapData_SO mapData)
        {
            foreach (TileProperty tileProperty in mapData.tileProperties)
            {
                TileDetails tileDetails = new TileDetails
                {
                    gridX = tileProperty.tileCoordinate.x,
                    gridY = tileProperty.tileCoordinate.y,
                };
                string key=tileDetails.gridX+"x"+tileDetails.gridY+"y"+mapData.sceneName;
                if (GetTileDetails(key) != null)
                {
                    tileDetails = GetTileDetails(key);
                }

                switch (tileProperty.gridType)
                {
                    case GridType.Diggable:
                        tileDetails.canDig = tileProperty.boolTypeValue;
                        break;
                    case GridType.DropItem:
                        tileDetails.canDropItem = tileProperty.boolTypeValue;
                        break;
                    case GridType.PlaceFurniture:
                        tileDetails.canPlaceFurniture = tileProperty.boolTypeValue;
                        break;
                    case GridType.NPCObstacle:
                        tileDetails.isNPCObstacle = tileProperty.boolTypeValue;
                        break;
                }

                if (GetTileDetails(key) != null)
                {
                    tileDetailsDict[key] = tileDetails;
                }
                else
                {
                    tileDetailsDict.Add(key, tileDetails);
                }
            }
        }

        /// <summary>
        /// 根据key返回瓦片信息
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public TileDetails GetTileDetails(string key)
        {
            if (tileDetailsDict.ContainsKey(key))
            {
                return tileDetailsDict[key];
            }
            return null;
        }

        /// <summary>
        /// 根据鼠标网格坐标返回瓦片信息
        /// </summary>
        /// <param name="mouseGridPos">鼠标网格坐标</param>
        /// <returns></returns>
        public TileDetails GetTileDetailsOnMousePosition(Vector3Int mouseGridPos)
        {
            string key=mouseGridPos.x+"x"+mouseGridPos.y+"y"+SceneManager.GetActiveScene().name;
            return GetTileDetails(key);
        }
        
        /// <summary>
        /// 执行实际工具或物品功能
        /// </summary>
        /// <param name="mouseWorldPos"></param>
        /// <param name="itemDetails"></param>
        private void OnExecuteActionAfterAnimation(Vector3 mouseWorldPos, ItemDetails itemDetails)
        {
            var mouseGridPos = currentGrid.WorldToCell(mouseWorldPos);
            var currentTile = GetTileDetailsOnMousePosition(mouseGridPos);
            if (currentTile != null)
            {
                Crop currentCrop = GetCropObject(mouseWorldPos);
                //物品使用实际功能
                switch (itemDetails.ItemType)
                {
                    case ItemType.Seed:
                        EventHandler.CallPlantSeedEvent(itemDetails.itemID, currentTile);
                        EventHandler.CallDropItemEvent(itemDetails.itemID, mouseWorldPos, itemDetails.ItemType);
                        EventHandler.CallPlaySoundEvent(SoundName.Plant);
                        break;
                    case ItemType.Commodity:
                        EventHandler.CallDropItemEvent(itemDetails.itemID, mouseWorldPos,itemDetails.ItemType);
                        break;
                    case ItemType.HoeTool:
                        SetDigGround(currentTile);
                        currentTile.daysSinceDug = 0;
                        currentTile.canDig = false;
                        currentTile.canDropItem = false;
                        EventHandler.CallPlaySoundEvent(SoundName.Hoe);
                        break;
                    case ItemType.WaterTool:
                        SetWaterGround(currentTile);
                        currentTile.daysSinceWatered = 0;
                        EventHandler.CallPlaySoundEvent(SoundName.Water);
                        break;
                    case ItemType.BreakTool:
                    case ItemType.ChopTool:
                        currentCrop?.ProcessToolAction(itemDetails,currentCrop.tileDetails);
                        break;
                    case ItemType.CollectTool:
                        //执行收割方法
                        currentCrop.ProcessToolAction(itemDetails,currentTile);
                        break;
                    case ItemType.ReapTool:
                        var reapCount = 0;
                        for (int i = 0; i < itemsInRadius.Count; i++)
                        {
                            EventHandler.CallParticleEffectEvent(ParticaleEffectType.ReapableScenery, itemsInRadius[i].transform.position+Vector3.up);
                            itemsInRadius[i].SpawnHarvestItems();
                            Destroy(itemsInRadius[i].gameObject);
                            reapCount++;
                            if (reapCount >= Settings.reapAmount)
                            {
                                break;
                            }
                        }
                        EventHandler.CallPlaySoundEvent(SoundName.Reap);
                        break;
                    case ItemType.Furniture:
                        //在地图上生成物品 itemmanager
                        //移除图纸 inventorymanager
                        //移除资源物品 inventorymanager
                        EventHandler.CallBuildFurnitureEvent(itemDetails.itemID,mouseWorldPos);
                        break;
                }
                UpdateTileDetails(currentTile);
            }
        }

        /// <summary>
        /// 物理方法判断鼠标点击位置作物
        /// </summary>
        /// <param name="mouseWorldPos"></param>
        /// <returns></returns>
        public Crop GetCropObject(Vector3 mouseWorldPos)
        {
            Collider2D[] colliders = Physics2D.OverlapPointAll(mouseWorldPos);
            Crop currentCrop = null;
            for (int i = 0; i < colliders.Length; i++)
            {
                if (colliders[i].GetComponent<Crop>())
                {
                    currentCrop = colliders[i].GetComponent<Crop>();
                }
            }
            return currentCrop;
        }

        /// <summary>
        /// 返回工具范围内的杂草
        /// </summary>
        /// <param name="tool"></param>
        /// <returns></returns>
        public bool HaveReapableItemsInRadius(Vector3 mouseWorldPos,ItemDetails tool)
        {
            itemsInRadius = new List<ReapItem>();

            Collider2D[] colliders = new Collider2D[20];
            Physics2D.OverlapCircleNonAlloc(mouseWorldPos,tool.itemUseRadius,colliders);
            if (colliders.Length > 0)
            {
                for (int i = 0; i < colliders.Length; i++)
                {
                    if (colliders[i] != null)
                    {
                        if (colliders[i].GetComponent<ReapItem>())
                        {
                            var item=colliders[i].GetComponent<ReapItem>();
                            itemsInRadius.Add(item);
                        }

                    }
                }
            }
            return itemsInRadius.Count > 0;
        }
        
        /// <summary>
        /// 显示挖坑瓦片
        /// </summary>
        /// <param name="tile"></param>
        private void SetDigGround(TileDetails tile)
        {
            Vector3Int pos = new Vector3Int(tile.gridX, tile.gridY, 0);
            if (digTilemap != null)
                digTilemap.SetTile(pos, digTile);
        }
        /// <summary>
        /// 显示浇水瓦片
        /// </summary>
        /// <param name="tile"></param>
        private void SetWaterGround(TileDetails tile)
        {
            Vector3Int pos = new Vector3Int(tile.gridX, tile.gridY, 0);
            if (waterTilemap != null)
                waterTilemap.SetTile(pos, waterTile);
        }

        /// <summary>
        /// 更新瓦片信息
        /// </summary>
        /// <param name="tileDetails"></param>
        public void UpdateTileDetails(TileDetails tileDetails)
        {
            string key=tileDetails.gridX+"x"+tileDetails.gridY+"y"+SceneManager.GetActiveScene().name;
            if (tileDetailsDict.ContainsKey(key))
            {
                tileDetailsDict[key] = tileDetails;
            }
            else
            {
                tileDetailsDict.Add(key, tileDetails);
            }
        }

        /// <summary>
        /// 刷新地图
        /// </summary>
        private void RefreshMap()
        {
            if(digTilemap!=null)
                digTilemap.ClearAllTiles();
            if(waterTilemap!=null)
                waterTilemap.ClearAllTiles();
            foreach (var crop in FindObjectsOfType<Crop>())
            {
                Destroy(crop.gameObject);
            }
            DisplayMap(SceneManager.GetActiveScene().name);
        }
        
        /// <summary>
        /// 显示地图瓦片
        /// </summary>
        /// <param name="sceneName"></param>
        private void DisplayMap(string sceneName)
        {
            foreach (var tile in tileDetailsDict)
            {
                var   key=tile.Key;
                var   tileDetails=tile.Value;

                if (key.Contains(sceneName))
                {
                    if(tileDetails.daysSinceDug>-1)
                        SetDigGround(tileDetails);
                    if(tileDetails.daysSinceWatered>-1)
                        SetWaterGround(tileDetails);
                    if(tileDetails.seedItemID>-1)
                    {
                        EventHandler.CallPlantSeedEvent(tileDetails.seedItemID,tileDetails);
                    }
                }
            }
        }

        /// <summary>
        /// 根据场景名称构建网格范围 输出范围和原点
        /// </summary>
        /// <param name="sceneName">场景名称</param>
        /// <param name="gridDimensions">网格范围</param>
        /// <param name="gridOrigin">网格原点</param>
        /// <returns>是否有当前场景信息</returns>
        public bool GetGridDimensions(string sceneName,out Vector2Int gridDimensions,out Vector2Int gridOrigin)
        {
            gridDimensions=Vector2Int.zero;
            gridOrigin = Vector2Int.zero;

            foreach (var mapData in MapDataList)
            {
                if (mapData.sceneName == sceneName)
                {
                    gridDimensions.x=mapData.gridWidth;
                    gridDimensions.y=mapData.gridHeight;
                    
                    gridOrigin.x=mapData.originX;
                    gridOrigin.y=mapData.originY;
                    
                    return true;
                }
            }
            return false;
        }
        
    }
}

