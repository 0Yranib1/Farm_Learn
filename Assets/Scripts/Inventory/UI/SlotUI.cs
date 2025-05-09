using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MFarm.Inventory
{

    public class SlotUI : MonoBehaviour,IPointerClickHandler,IBeginDragHandler, IDragHandler,IEndDragHandler
    {
        [Header("组件获取")]
        //在unity中可赋值变量
        [SerializeField]
        private Image slotImage;

        [SerializeField] private TextMeshProUGUI amountText;
        [SerializeField] public Image slotHighlight;
        [SerializeField] private Button button;

        public SlotType SlotType;

        public bool isSelected;

        public ItemDetails itemDetails;
        public int itemAmount;
        public  int slotIndex;

        public InventoryLocation Location
        {
            get
            {
                return SlotType switch
                {
                    SlotType.Bag => InventoryLocation.Player,
                    SlotType.Box => InventoryLocation.Box,
                    _ => InventoryLocation.Player,
                };
            }
        }
        
        public InventoryUI inventoryUI => GetComponentInParent<InventoryUI>();
        
        void Start()
        {
            isSelected = false;
            if (itemDetails == null)
            {
                updateEmptySlot();
            }
        }

        //更新格子UI和信息
        public void updateSlot(ItemDetails item, int amount)
        {
            itemDetails = item;
            slotImage.sprite = item.itemIcon;
            slotImage.enabled = true;
            itemAmount = amount;
            amountText.text = amount.ToString();
            button.interactable = true;
        }

        //讲物品格更新为空样式
        public void updateEmptySlot()
        {
            if (isSelected)
            {
                isSelected = false;
                inventoryUI.UpdateSlotHighlight(-1);
                EventHandler.CallItemSelectEvent(itemDetails, isSelected);
            }

            itemDetails = null;
            slotImage.enabled = false;
            amountText.text = string.Empty;
            button.interactable = false;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if(itemDetails==null)
                return;
            isSelected=!isSelected;
            inventoryUI.UpdateSlotHighlight(slotIndex);

            if (SlotType == SlotType.Bag)
            {
                //通知物品被选中的状态和信息
                EventHandler.CallItemSelectEvent(itemDetails, isSelected);
            }
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (itemAmount != 0)
            {
                inventoryUI.dragItem.enabled = true;
                inventoryUI.dragItem.sprite = slotImage.sprite;
                inventoryUI.dragItem.SetNativeSize();
                isSelected = true;
                inventoryUI.UpdateSlotHighlight(slotIndex);
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            inventoryUI.dragItem.transform.position = Input.mousePosition;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            inventoryUI.dragItem.enabled = false;
            // Debug.Log(eventData.pointerCurrentRaycast.gameObject);
            if (eventData.pointerCurrentRaycast.gameObject != null)
            {
                if(eventData.pointerCurrentRaycast.gameObject.GetComponent<SlotUI>()==null)
                    return;
                var targetSlot=eventData.pointerCurrentRaycast.gameObject.GetComponent<SlotUI>();
                int targetIndex = targetSlot.slotIndex;
                //在玩家自身背包内实现交换
                if (SlotType == SlotType.Bag && targetSlot.SlotType == SlotType.Bag)
                {
                    InventoryManager.Instance.SwapItem(slotIndex, targetIndex);
                }else if (SlotType==SlotType.Shop && targetSlot.SlotType == SlotType.Bag)//买
                {
                    EventHandler.CallShowTradeUI(itemDetails,false);
                }else if (SlotType == SlotType.Bag && targetSlot.SlotType == SlotType.Shop) //卖
                {
                    EventHandler.CallShowTradeUI(itemDetails,true);
                }else if (SlotType != SlotType.Shop && targetSlot.SlotType != SlotType.Shop &&
                          SlotType != targetSlot.SlotType)
                {
                    //夸背包数据交换物品
                    InventoryManager.Instance.SwapItem(Location,slotIndex,targetSlot.Location,targetSlot.slotIndex);
                }
                inventoryUI.UpdateSlotHighlight(-1);
            }
            // else
            // {
            //     //测试扔在地上
            //     //鼠标对应世界地图坐标
            //     if (itemDetails.canDropped)
            //     {
            //         var pos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y,
            //             -Camera.main.transform.position.z));
            //         EventHandler.CallInstantiateItemInScene(itemDetails.itemID, pos);
            //     }
            //
            // }
        }
    }
}
