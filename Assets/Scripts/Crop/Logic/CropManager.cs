using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MFarm.CropPlant
{
    public class CropManager : Singleton<CropManager>
    {
        public CropDataList_SO cropData;
        private Transform cropParent;
        private Grid currentGrid;
        private Season currentSeason;

        private void OnEnable()
        {
            EventHandler.PlantSeedEvent += OnPlantSeedEvent;
            EventHandler.AfterSceneLoadEvent += OnAfterSceneLoadedEvent;
            EventHandler.GameDayEvent += OnGameDayEvent;
        }

        private void OnDisable()
        {
            EventHandler.PlantSeedEvent -= OnPlantSeedEvent;
            EventHandler.AfterSceneLoadEvent -= OnAfterSceneLoadedEvent;
            EventHandler.GameDayEvent -= OnGameDayEvent;
        }
        private void OnGameDayEvent(int day, Season season)
        {
            currentSeason = season;
        }
        private void OnAfterSceneLoadedEvent()
        {
            currentGrid = FindObjectOfType<Grid>();
            cropParent = GameObject.FindWithTag("CropParent").transform;
        }
        private void OnPlantSeedEvent(int seedItemID, TileDetails tileDetails)
        {
            CropDetails currentCrop= getCropDetails(seedItemID);
            if (currentCrop != null && SeasonAvailable(currentCrop) && tileDetails.seedItemID == -1) //种植
            {
                tileDetails.seedItemID= seedItemID;
                tileDetails.growthDays = 0;
                //显示农作物
                DisplayCropPlant(tileDetails, currentCrop);
            }
            else if(tileDetails.seedItemID!=-1)//刷新地图
            {
                //显示农作物
                DisplayCropPlant(tileDetails, currentCrop);
            }
        }

        /// <summary>
        /// 显示作物
        /// </summary>
        /// <param name="tileDetails"></param>
        /// <param name="cropDetails"></param>
        private void DisplayCropPlant(TileDetails tileDetails, CropDetails cropDetails)
        {
            //成长阶段
            int growthStages = cropDetails.growthDays.Length;
            int currentStage = 0;
            int dayCounter = cropDetails.TotalGrowthDays;
            //倒序计算当前成长阶段
            for (int i = growthStages - 1; i >= 0; i--)
            {
                if (tileDetails.growthDays>=dayCounter)
                {
                    currentStage = i;
                    break;
                }
                dayCounter -= cropDetails.growthDays[i];
            }
            //获取当前阶段的prefab
            GameObject growthPrefab = cropDetails.growthPrefabs[currentStage];
            Sprite cropSprite= cropDetails.growthSprites[currentStage];
            Vector3 pos = new Vector3(tileDetails.gridX + 0.5f, tileDetails.gridY + 0.5f, 0);
            
            GameObject cropInstance = Instantiate(growthPrefab, pos, Quaternion.identity, cropParent);
            
            cropInstance.GetComponentInChildren<SpriteRenderer>().sprite = cropSprite;

            cropInstance.GetComponent<Crop>().cropDetails = cropDetails;
            cropInstance.GetComponent<Crop>().tileDetails = tileDetails;

        }
        
        /// <summary>
        /// 通过物品ID查找种子信息
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public CropDetails getCropDetails(int ID)
        {
            return cropData.cropDetailsList.Find(x => x.seedItemID == ID);
        }

        /// <summary>
        /// 判断当前季节是否可以种植
        /// </summary>
        /// <param name="crop"></param>
        /// <returns></returns>
        private bool SeasonAvailable(CropDetails crop)
        {
            for (int i = 0; i < crop.seasons.Length; i++)
            {
                if (crop.seasons[i] == currentSeason)
                    return true;
            }
            return false;
        }
    }
}
