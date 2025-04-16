using System.Collections;
using System.Collections.Generic;
using MFarm.Inventory;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ItemTooltip : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private TextMeshProUGUI typeText;
    [SerializeField] private Text valueText;
    [SerializeField] private GameObject bottomPart;
    [Header("建造")] 
    [SerializeField] public GameObject resourcePanel;

    [SerializeField] private Image[] resourceItem;

    public void SetupTooltip(ItemDetails itemDetails, SlotType slotType)
    {
        nameText.text = itemDetails.itemName;
        descriptionText.text = itemDetails.itemDescription;
        typeText.text = GetItemType(itemDetails.ItemType);
        if(itemDetails.ItemType==ItemType.Seed || itemDetails.ItemType==ItemType.Commodity || itemDetails.ItemType==ItemType.Furniture)
        {
            bottomPart.SetActive(true);
            var price = itemDetails.itemPrice;
            if (slotType == SlotType.Bag)
            {
                price = (int)(price * itemDetails.sellPercentage);
            }
            valueText.text = price.ToString();
        }
        else
        {
            bottomPart.SetActive(false);
        }
        LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
    }
    private string GetItemType(ItemType itemType)
    {
        return itemType switch
        {
            ItemType.Seed => "种子",
            ItemType.Commodity => "商品",
            ItemType.Furniture => "家具",
            ItemType.BreakTool => "工具",
            ItemType.ChopTool => "工具",
            ItemType.CollectTool => "工具",
            ItemType.HoeTool => "工具",
            ItemType.ReapTool => "工具",
            ItemType.WaterTool => "工具",
            _ => "无"
        };
    }
    
    public void SetupResourcePanel(int ID)
    {
        var bluePrintDetails = InventoryManager.Instance.bluePrintData.GetBluePrintDetails(ID);
        for (int i = 0; i < resourceItem.Length; i++)
        {
            if (i < bluePrintDetails.resourceItem.Length)
            {
                resourceItem[i].gameObject.SetActive(true);
                resourceItem[i].sprite = InventoryManager.Instance.GetItemDetails(bluePrintDetails.resourceItem[i].itemID).itemIcon;
                resourceItem[i].transform.GetChild(0).GetComponent<Text>().text=bluePrintDetails.resourceItem[i].itemAmount.ToString();
                
            }
            else
            {
                resourceItem[i].gameObject.SetActive(false);
            }
        }
    }
    
}
