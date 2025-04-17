using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace MFarm.Inventory
{
    public class Box : MonoBehaviour
    {
        public InventoryBag_SO boxBagTemplate;
        public InventoryBag_SO boxBagData;
        public GameObject mouseIcon;
        private bool canOpen = false;
        private bool isOpen;
        public int index;

        private void OnEnable()
        {
            if (boxBagData == null)
            {
                boxBagData = Instantiate(boxBagTemplate);
            }

            // var key = this.name + index;
            // if (InventoryManager.Instance.GetBoxDataList(key) != null)//刷新地图读取数据
            // {
            //     boxBagData.itemList = InventoryManager.Instance.GetBoxDataList(key);
            // }
            // else//新建箱子
            // {
            //     if (index == 0)
            //     {
            //         index = InventoryManager.Instance.BoxDataAmount;
            //     }
            //     InventoryManager.Instance.AddBoxDataDict(this);
            // }
        }
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                canOpen = true;
                mouseIcon.SetActive(true);
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                canOpen = false;
                mouseIcon.SetActive(false);
            }
        }

        private void Update()
        {
            if (!isOpen && Input.GetMouseButtonDown(1) && canOpen)
            {
                EventHandler.CallBaseBagOpenEvent(SlotType.Box,boxBagData);
                isOpen = true;
            }

            if (!canOpen && isOpen)
            {
                //关闭箱子
                EventHandler.CallBaseBagCloseEvent(SlotType.Box,boxBagData);
                isOpen = false;
            }
            
            if (Input.GetKeyDown(KeyCode.Escape) && isOpen)
            {
                //关闭箱子
                EventHandler.CallBaseBagCloseEvent(SlotType.Box,boxBagData);
                isOpen = false;
            }
            
        }

        public void InitBox(int boxIndex)
        {
            index=boxIndex;
            var key = this.name + index;
            if (InventoryManager.Instance.GetBoxDataList(key) != null)//刷新地图读取数据
            {
                boxBagData.itemList = InventoryManager.Instance.GetBoxDataList(key);
            }
            else//新建箱子
            {
                InventoryManager.Instance.AddBoxDataDict(this);
            }
        }
        
        
    }
}

