using System;
using System.Collections;
using System.Collections.Generic;
using MFarm.Save;
using UnityEngine;

namespace MFarm.Inventory
{
    public class InventoryManager : Singleton<InventoryManager>,ISaveable
    {

        public ItemDataList_SO ItemDataListSo;
        [Header("建造蓝图")]
        public BluePrintDataList_SO bluePrintData;
        
        public InventoryBag_SO playerBag;

        private InventoryBag_SO currentBoxBag;
        [Header("交易")] 
        public int playerMoney;
        private Dictionary<string,List<InventoryItem>> boxDataDict=new Dictionary<string, List<InventoryItem>>();
        public int BoxDataAmount=> boxDataDict.Count;
        
        public string GUID => GetComponent<DataGUID>().guid;
        //通过ID获得物品信息
        private void Start()
        {
            EventHandler.CallUpdateInventoryUI(InventoryLocation.Player, playerBag.itemList);
            
            ISaveable saveable = this;
            saveable.RegisterSaveable();
        }

        private void OnEnable()
        {
            EventHandler.DropItemEvent += OnDropItemEvent;
            EventHandler.HarvestAtPlayerPosition += OnHarvestAtPlayerPositionEvent;
            EventHandler.BuildFurnitureEvent += OnBuildFurnitureEvent;
            EventHandler.BaseBagOpenEvent += OnBaseBagOpenEvent;
        }

        private void OnDisable()
        {
            EventHandler.DropItemEvent -= OnDropItemEvent;
            EventHandler.HarvestAtPlayerPosition -= OnHarvestAtPlayerPositionEvent;
            EventHandler.BuildFurnitureEvent -= OnBuildFurnitureEvent;
            EventHandler.BaseBagOpenEvent -= OnBaseBagOpenEvent;
        }

        private void OnBaseBagOpenEvent(SlotType slotType, InventoryBag_SO bag_So)
        {
            currentBoxBag= bag_So;
        }

        private void OnBuildFurnitureEvent(int ID, Vector3 mousePos)
        {
            RemoveItem(ID,1);
            BluePrintDetails bluePrint = bluePrintData.GetBluePrintDetails(ID);
            foreach (var item in bluePrint.resourceItem)
            {
                RemoveItem(item.itemID, item.itemAmount);
            }
        }

        private void OnHarvestAtPlayerPositionEvent(int ID)
        {
            
            //背包是否有同id物品
            var index = GetItemIndexInBag(ID);

            AddItemAtIndex(ID, index, 1);
            
            //更新UI
            EventHandler.CallUpdateInventoryUI(InventoryLocation.Player, playerBag.itemList);
        }
        
        private void OnDropItemEvent(int ID, Vector3 pos, ItemType itemType)
        {
            RemoveItem(ID, 1);
        }
        
        public ItemDetails GetItemDetails(int ID)
        {
            return ItemDataListSo.itemDetailsList.Find(x => x.itemID == ID);
        }
        //添加物品
        public void AddItem(Item item,bool toDestory)
        {
            //背包是否有同id物品
            var index = GetItemIndexInBag(item.itemID);

            AddItemAtIndex(item.itemID, index, 1);
            
            if (toDestory)
            {
                Destroy(item.gameObject);
            }
            //更新UI
            EventHandler.CallUpdateInventoryUI(InventoryLocation.Player, playerBag.itemList);
        }
        //检查是否有相同id物品
        private int GetItemIndexInBag(int ID)
        {
            for (int i = 0; i < playerBag.itemList.Count; i++)
            {
                if (playerBag.itemList[i].itemID == ID)
                {
                    return i;
                }
            }

            return -1;
        }
        //检查背包是否还有空位
        public bool CheckBagCapacity()
        {
            for (int i = 0; i < playerBag.itemList.Count; i++)
            {
                if (playerBag.itemList[i].itemID==0)
                {
                    return  true;
                }
            }
            return false;
        }

        //指定位置添加物品
        private void AddItemAtIndex(int ID, int index, int amount)
        {
            //背包内没有相同id物品 有空位
            if (index == -1)
            {
                var item=new InventoryItem{
                    itemID=ID,
                    itemAmount=amount
                };
                for (int i = 0; i < playerBag.itemList.Count; i++)
                {
                    if (playerBag.itemList[i].itemID==0)
                    {
                        playerBag.itemList[i] = item;
                        break;
                    }
                }
            }
            //背包内已有相同id物品
            else
            {
                int currentAmount=playerBag.itemList[index].itemAmount+amount;
                var item = new InventoryItem
                {
                    itemID = ID,
                    itemAmount = currentAmount
                };
                playerBag.itemList[index] = item;
            }
        }
        
        /// <summary>
        /// 交换物品位置
        /// </summary>
        /// <param name="fromIndex">起始序号</param>
        /// <param name="targetIndex">目标数据序号</param>
        public void SwapItem(int fromIndex, int targetIndex)
        {
            InventoryItem currentItem=playerBag.itemList[fromIndex];
            InventoryItem targetItem=playerBag.itemList[targetIndex];
            if (targetItem.itemID != 0)
            {
                playerBag.itemList[fromIndex] = targetItem;
                playerBag.itemList[targetIndex] = currentItem;
            }
            else
            {
                playerBag.itemList[targetIndex] = currentItem;
                playerBag.itemList[fromIndex] = new InventoryItem();
            }
            EventHandler.CallUpdateInventoryUI(InventoryLocation.Player, playerBag.itemList);
        }

        /// <summary>
        /// 跨背包交换数据
        /// </summary>
        /// <param name="locationFrom"></param>
        /// <param name="fromIndex"></param>
        /// <param name="locationTarget"></param>
        /// <param name="targetIndex"></param>
        public void SwapItem(InventoryLocation locationFrom, int fromIndex, InventoryLocation locationTarget,
            int targetIndex)
        {
            var currentList = GetItemList(locationFrom);
            var targetList= GetItemList(locationTarget);
            
            InventoryItem currentItem=currentList[fromIndex];
            if (targetIndex < targetList.Count)
            {
                InventoryItem targetItem=targetList[targetIndex];

                if (targetItem.itemID != 0 && targetItem.itemID != currentItem.itemID) //有不相同的物品
                {
                    currentList[fromIndex] = targetItem;
                    targetList[targetIndex] = currentItem;
                }else if (currentItem.itemID == targetItem.itemID)//目标格为同一物品
                {
                    targetItem.itemAmount += currentItem.itemAmount;
                    targetList[targetIndex] = targetItem;
                    currentList[fromIndex] = new InventoryItem();
                }
                else //目标为空格
                {
                    targetList[targetIndex] = currentItem;
                    currentList[fromIndex] = new InventoryItem();
                }
                EventHandler.CallUpdateInventoryUI(locationFrom, currentList);
                EventHandler.CallUpdateInventoryUI(locationTarget, targetList);
            }
        }
        
        /// <summary>
        /// 根据位置返回背包数据列表
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        private List<InventoryItem> GetItemList(InventoryLocation location)
        {
            return location switch
            {
                InventoryLocation.Player => playerBag.itemList,
                InventoryLocation.Box => currentBoxBag.itemList,
                _ => null
            };
        }

        /// <summary>
        /// 移除指定数量背包物品
        /// </summary>
        /// <param name="ID">物品ID</param>
        /// <param name="removeAmount"></param>
        private void RemoveItem(int ID, int removeAmount)
        {
            var index = GetItemIndexInBag(ID);
            if (playerBag.itemList[index].itemAmount > removeAmount)
            {
                var amount=playerBag.itemList[index].itemAmount-removeAmount;
                var item=new InventoryItem
                {
                    itemID = ID,
                    itemAmount = amount
                };
                playerBag.itemList[index] = item;
            }else if (playerBag.itemList[index].itemAmount == removeAmount)
            {
                var item = new InventoryItem();
                playerBag.itemList[index] = item;
            }
            EventHandler.CallUpdateInventoryUI(InventoryLocation.Player, playerBag.itemList);
        }

        /// <summary>
        /// 交易物品
        /// </summary>
        /// <param name="itemDetails"></param>
        /// <param name="amount"></param>
        /// <param name="isSellTrade"></param>
        public void TradeItem(ItemDetails itemDetails,int amount,bool isSellTrade)
        {
            int cost=itemDetails.itemPrice*amount;
            //获得物品背包位置
            int index = GetItemIndexInBag(itemDetails.itemID);
            if (isSellTrade)//卖
            {
                if (playerBag.itemList[index].itemAmount >= amount)
                {
                    RemoveItem(itemDetails.itemID, amount);
                    //卖出总价
                    cost= (int)(cost * itemDetails.sellPercentage);
                    playerMoney += cost;
                }
            }
            else if(playerMoney-cost>=0)
            {
                if (CheckBagCapacity())
                {
                    AddItemAtIndex(itemDetails.itemID, index, amount);
                }
                playerMoney -= cost;
            }
            //刷新ui
            EventHandler.CallUpdateInventoryUI(InventoryLocation.Player, playerBag.itemList);
        }

        /// <summary>
        /// 检查建造资源物品库存
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public bool CheckStock(int ID)
        {
            var bluePrintDetails = bluePrintData.GetBluePrintDetails(ID);

            foreach (var resourceItem in bluePrintDetails.resourceItem)
            {
                var itemStock = playerBag.GetInventoryItem(resourceItem.itemID);
                if (itemStock.itemAmount >= resourceItem.itemAmount)
                {
                    continue;
                }
                else
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// 查找箱子数据
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public List<InventoryItem> GetBoxDataList(string key)
        {
            if(boxDataDict.ContainsKey(key))
                return boxDataDict[key];
            else
                return null;
        }

        /// <summary>
        /// 加入箱子数据字典
        /// </summary>
        /// <param name="box"></param>
        public void AddBoxDataDict(Box box)
        {
            var key=box.name+box.index;
            if (!boxDataDict.ContainsKey(key))
            {
                boxDataDict.Add(key, box.boxBagData.itemList);
            }
        }


        public GameSaveData generateSaveData()
        {
            GameSaveData saveData = new GameSaveData();
            saveData.playerMoney = this.playerMoney;

            saveData.inventoryDict = new Dictionary<string, List<InventoryItem>>();
            saveData.inventoryDict.Add(playerBag.name, playerBag.itemList);
            foreach (var item in boxDataDict)
            {
                saveData.inventoryDict.Add(item.Key, item.Value);
            }
            return saveData;
        }

        public void RestoreData(GameSaveData saveData)
        {
            this.playerMoney = saveData.playerMoney;
            playerBag.itemList = saveData.inventoryDict[playerBag.name];
            foreach (var item in saveData.inventoryDict)
            {
                if (boxDataDict.ContainsKey(item.Key))
                {
                    boxDataDict[item.Key] = item.Value;
                }
            }
            EventHandler.CallUpdateInventoryUI(InventoryLocation.Player, playerBag.itemList);
            
            
        }
    }
}

