using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MFarm.Inventory
{
    public class InventoryManager : Singleton<InventoryManager>
    {

        public ItemDataList_SO ItemDataListSo;

        public InventoryBag_SO playerBag;

        [Header("交易")] 
        public int playerMoney;
        //通过ID获得物品信息
        private void Start()
        {
            EventHandler.CallUpdateInventoryUI(InventoryLocation.Player, playerBag.itemList);
        }

        private void OnEnable()
        {
            EventHandler.DropItemEvent += OnDropItemEvent;
            EventHandler.HarvestAtPlayerPosition += OnHarvestAtPlayerPositionEvent;
        }

        private void OnDisable()
        {
            EventHandler.DropItemEvent -= OnDropItemEvent;
            EventHandler.HarvestAtPlayerPosition -= OnHarvestAtPlayerPositionEvent;
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
        
    }
}

