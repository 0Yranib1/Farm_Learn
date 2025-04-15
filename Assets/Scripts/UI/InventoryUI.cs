using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MFarm.Inventory
{
    public class InventoryUI : MonoBehaviour
    {
        public ItemTooltip ItemTooltip;
        [Header("拖拽图片")] public Image dragItem;
        [Header("玩家背包UI")]
        [SerializeField] private GameObject bagUI;
        private bool bagOpened;

        [Header("通用背包")] 
        [SerializeField]
        private GameObject baseBag;

        public GameObject shopSlotPrefab;
        
        [SerializeField] private SlotUI[] playerSlots;
        [SerializeField] private List<SlotUI> baseBagSlots;
        void Start()
        {
            //给背包数据编号赋值
            for (int i = 0; i < playerSlots.Length; i++)
            {
                playerSlots[i].slotIndex = i;
            }

            bagOpened = bagUI.activeInHierarchy;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.B))
            {
                openBagUI();
            }
        }

        private void OnEnable()
        {
            EventHandler.UpdateInventoryUI += OnUpdateInventoryUI;
            EventHandler.BeforeSceneUnloadEvent += OnBeforeSceneUnloadedEvent;
            EventHandler.BaseBagOpenEvent += OnBaseBagOpenEvent;
        }

        private void OnDisable()
        {
            EventHandler.UpdateInventoryUI -= OnUpdateInventoryUI;
            EventHandler.BeforeSceneUnloadEvent -= OnBeforeSceneUnloadedEvent;
            EventHandler.BaseBagOpenEvent -= OnBaseBagOpenEvent;
        }
        private void OnBeforeSceneUnloadedEvent()
        {
            UpdateSlotHighlight(-1);
        }

        private void OnBaseBagOpenEvent(SlotType slotType, InventoryBag_SO bagData)
        {
            GameObject prefab = slotType switch
            {
                SlotType.Shop => shopSlotPrefab,
                _ => null,
            };
            //生成背包ui
            baseBag.SetActive(true);
            baseBagSlots = new List<SlotUI>();
            for (int i = 0; i < bagData.itemList.Count; i++)
            {
                var slot = Instantiate(prefab, baseBag.transform.GetChild(0)).GetComponent<SlotUI>();
                slot.slotIndex = i;
                baseBagSlots.Add(slot);
            }
            LayoutRebuilder.ForceRebuildLayoutImmediate(baseBag.GetComponent<RectTransform>());
            //更新ui
            OnUpdateInventoryUI(InventoryLocation.Box,bagData.itemList);
        }
        
        public void OnUpdateInventoryUI(InventoryLocation location,List<InventoryItem> list)
        {
            switch (location)
            {
                case  InventoryLocation.Player:
                    for (int i = 0; i < playerSlots.Length; i++)
                    {
                        if (list[i].itemAmount > 0)
                        {
                            playerSlots[i].updateSlot(InventoryManager.Instance.GetItemDetails(list[i].itemID),list[i].itemAmount);
                        }else
                        {
                            playerSlots[i].updateEmptySlot();
                        }
                    }    
                    break;
                case InventoryLocation.Box:
                    for (int i = 0; i < baseBagSlots.Count; i++)
                    {
                        if (list[i].itemAmount > 0)
                        {
                            baseBagSlots[i].updateSlot(InventoryManager.Instance.GetItemDetails(list[i].itemID),list[i].itemAmount);
                        }else
                        {
                            baseBagSlots[i].updateEmptySlot();
                        }
                    }    
                    break;
            }
        }

        //打开关闭背包UI 按钮事件
        public void openBagUI()
        {
            bagOpened=!bagOpened;
            bagUI.SetActive(bagOpened);
        }

        //更新高亮显示
        public void UpdateSlotHighlight(int index)
        {
            foreach (var slot in playerSlots)
            {
                if (slot.isSelected && slot.slotIndex == index)
                {
                    slot.slotHighlight.gameObject.SetActive(true);
                }
                else
                {
                    slot.isSelected = false;
                    slot.slotHighlight.gameObject.SetActive(false);
                }
            }
        }
        
    }
    
}

