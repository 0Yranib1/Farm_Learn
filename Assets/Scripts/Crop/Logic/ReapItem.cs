using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MFarm.CropPlant
{
    public class ReapItem : MonoBehaviour
    {
        private CropDetails cropDetails;
        private Transform playerTransform => FindObjectOfType<Player>().transform;

        public void InitCropData(int ID)
        {
            cropDetails = CropManager.Instance.getCropDetails(ID);
            
        }
        
        /// <summary>
        /// 生成收获物
        /// </summary>
        public void SpawnHarvestItems()
        {
            for (int i = 0; i < cropDetails.producedItemID.Length; i++)
            {
                int amountToProduce;

                if (cropDetails.producedMinAmount[i] == cropDetails.producedMaxAmount[i])
                {
                    //代表只生成指定数量的
                    amountToProduce = cropDetails.producedMinAmount[i];
                }
                else    //物品随机数量
                {
                    amountToProduce = Random.Range(cropDetails.producedMinAmount[i], cropDetails.producedMaxAmount[i] + 1);
                }

                //执行生成指定数量的物品
                for (int j = 0; j < amountToProduce; j++)
                {
                    if (cropDetails.generateAtPlayerPosition)
                        EventHandler.CallHarvestAtPlayerPosition(cropDetails.producedItemID[i]);
                    else
                    {
                        //判断生成物体方向
                        var dirX=transform.position.x>playerTransform.position.x?1:-1;
                        //一定范围内随机
                        var spawnPos =
                            new Vector3(transform.position.x + Random.Range(dirX, cropDetails.spawnRadius.x * dirX),
                                transform.position.y + Random.Range(-cropDetails.spawnRadius.y, cropDetails.spawnRadius.y),
                                0);
                        
                        EventHandler.CallInstantiateItemInScene(cropDetails.producedItemID[i], spawnPos);
                    }
                }
            }
            
        }
    }
}

