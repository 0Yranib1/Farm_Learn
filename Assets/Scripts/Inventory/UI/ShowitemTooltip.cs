using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MFarm.Inventory
{

    [RequireComponent(typeof(SlotUI))]
public class ShowitemTooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

    private SlotUI slotUI;
    private InventoryUI inventoryUI => GetComponentInParent<InventoryUI>();

    private void Awake()
    {
        slotUI = GetComponent<SlotUI>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (slotUI.itemDetails != null)
        {
            inventoryUI.ItemTooltip.gameObject.SetActive(true);
            inventoryUI.ItemTooltip.SetupTooltip(slotUI.itemDetails, slotUI.SlotType);
            inventoryUI.ItemTooltip.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0);
            inventoryUI.ItemTooltip.transform.position = transform.position + Vector3.up * 60;

            if (slotUI.itemDetails.ItemType == ItemType.Furniture)
            {
                inventoryUI.ItemTooltip.resourcePanel.SetActive(true);
                inventoryUI.ItemTooltip.SetupResourcePanel(slotUI.itemDetails.itemID);
            }
            else
            {
                inventoryUI.ItemTooltip.resourcePanel.SetActive(false);
            }
            
        }else
        {
            inventoryUI.ItemTooltip.gameObject.SetActive(false);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        inventoryUI.ItemTooltip.gameObject.SetActive(false);
    }


}

}